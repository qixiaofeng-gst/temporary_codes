# -*- coding: utf-8 -*-
"""
human VS AI models
Input your move in the format: 2,3

@author: Junxiao Song
"""

from __future__ import print_function
import pickle
from game import Board, Game
from mcts_pure import MCTSPlayer as MCTS_Pure
from mcts_alphaZero import MCTSPlayer
# from policy_value_net_numpy import PolicyValueNetNumpy
# from policy_value_net import PolicyValueNet  # Theano and Lasagne
# from policy_value_net_pytorch import PolicyValueNet  # Pytorch
from policy_value_net_tensorflow import PolicyValueNet # Tensorflow
# from policy_value_net_keras import PolicyValueNet  # Keras

import tkinter as tk
from tkinter import messagebox
import threading

mouse_click = '<Button-1>'
gui_cells = []

class GuiCell(object):
	def __init__(self, board_gui, board_mod, players, position):
		self.mami = tk.Frame(
			board_gui, bg='gray', bd=2,
			relief=tk.RIDGE, width=46, height=46
		)
		self.mami.bind(mouse_click, self.on_click)
		x, y = position
		self.mami.grid(column=x, row=y)
		self.board_gui = board_gui
		self.board_mod = board_mod
		self.players = players
		self.position = position
	
	def put_piece(self):
		self.mami['bg'] = (
			'black' if 1 == self.board_mod.get_current_player() else 'white'
		)
		self.mami.unbind(mouse_click)
	
	def on_click(self, event):
		self.put_piece()
		move = self.board_mod.location_to_move(list(self.position))
		self.board_mod.do_move(move)
		end, winner = self.board_mod.game_end()
		if end:
			messagebox.showinfo('Congratulations', 'You win!')
			self.board_gui.title('You win.')
			return
		def opponent_move():
			opponent = self.players[self.board_mod.get_current_player()]
			o_move = opponent.get_action(self.board_mod)
			put_piece((o_move // self.board_mod.width, o_move % self.board_mod.width))
			self.board_mod.do_move(o_move)
			end, winner = self.board_mod.game_end()
			self.board_gui.title('Your turn.')
			if end:
				messagebox.showinfo('Notification', 'You lose.')
				self.board_gui.title('You lose.')
		t = threading.Thread(target=opponent_move)
		t.start()
		self.board_gui.title('Opponent turn. Waiting...')

def put_piece(position):
	x, y = position
	gui_cells[x][y].put_piece()

class Human(object):
	"""
	human player
	"""

	def __init__(self):
		self.player = None

	def set_player_ind(self, p):
		self.player = p

	def get_action(self, board):
		try:
			location = input("Your move: ")
			if isinstance(location, str):  # for python3
				location = [int(n, 10) for n in location.split(",")]
			move = board.location_to_move(location)
		except Exception as e:
			move = -1
		if move == -1 or move not in board.availables:
			print("invalid move")
			move = self.get_action(board)
		return move

	def __str__(self):
		return "Human {}".format(self.player)

def run():
	width, height, n = 6, 6, 4
	try:
		board = Board(width=width, height=height, n_in_row=n)
		# ############### Human VS AI ###################
		# load the trained policy_value_net in either Theano/Lasagne, PyTorch or TensorFlow
		# load the provided model (trained in Theano/Lasagne) into a MCTS player written in pure numpy
		"""model_file = 'best_policy_8_8_5.model'
		try:
			policy_param = pickle.load(open(model_file, 'rb'))
		except:
			policy_param = pickle.load(open(model_file, 'rb'), encoding='bytes')  # To support python3
		best_policy = PolicyValueNetNumpy(width, height, policy_param)"""
		best_policy = PolicyValueNet(width, height, 'best_policy.model')
		# uncomment the following line to play with pure MCTS (it's much weaker even with a larger n_playout)
		# mcts_player = MCTS_Pure(c_puct=5, n_playout=1000)
		mcts_player = MCTSPlayer(best_policy.policy_value_fn, c_puct=5, n_playout=400)  # set larger n_playout for better performance
		# human player, input your move in the format: 2,3
		human = Human()
		
		board.init_board(0)
		p1, p2 = board.players
		human.set_player_ind(p1)
		mcts_player.set_player_ind(p2)
		players = {p1: human, p2: mcts_player}
		
		app_tk = tk.Tk()
		app_tk.resizable(False, False)
		app_tk.geometry('750x750+50+50')
		app_tk.title('Human VS AI - Gomoku')
		for x in range(width):
			cells_column = []
			gui_cells.append(cells_column)
			for y in range(height):
				cells_column.append(GuiCell(app_tk, board, players, (x, y)))

		app_tk.mainloop()
	except KeyboardInterrupt:
		print('\n\rquit')

if __name__ == '__main__':
  run()
