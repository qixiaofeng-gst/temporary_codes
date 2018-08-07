#!/user/bin/env python
# coding=<utf-8>
"""
human VS AI models
Input your move in the format: 2,3

@author: Junxiao Song
"""

from __future__ import print_function
from game import Board, Game
from mcts_alpha_zero import MCTSPlayer
from policy_value_net import PolicyValueNet

from tkinter import messagebox

import tkinter as tk
import threading
import argparse
import sys

mouse_click = '<Button-1>'
cell_size = 50
gui_cells = []
is_opponent_turn = False
human_first = False
piece_count = 0

class GuiCell(object):
	def __init__(self, board_gui, board_mod, players, position):
		self.mami = tk.Frame(
			board_gui, bg='gray', bd=2,
			relief=tk.RIDGE, width=cell_size, height=cell_size
		)
		self.mami.bind(mouse_click, self.on_click)
		x, y = position
		self.mami.grid(column=x, row=y)
		self.board_gui = board_gui
		self.board_mod = board_mod
		self.players = players
		self.position = position
	
	def put_piece(self):
		global piece_count
		self.mami['bg'] = (
			'black' if 0 == (piece_count % 2) else 'white'
		)
		piece_count += 1
		self.mami.unbind(mouse_click)
	
	def on_click(self, event):
		if is_opponent_turn:
			messagebox.showinfo('Be patient', 'Plz wait for AI\'s turn end.')
			return
		self.put_piece()
		move = self.board_mod.location_to_move(list(self.position))
		self.board_mod.do_move(move)
		end, winner = self.board_mod.game_end()
		if end:
			messagebox.showinfo('Congratulations', 'You win!')
			self.board_gui.title('You win.')
			end_game()
			return
		opponent_turn(self.board_gui, self.players, self.board_mod)

def opponent_turn(board_gui, players, board_mod):
	global is_opponent_turn
	is_opponent_turn = True
	board_gui.title('Opponent turn. Waiting...')
	def opponent_move():
		global is_opponent_turn
		opponent = players[board_mod.get_current_player()]
		o_move = opponent.get_action(board_mod)
		put_piece((o_move // board_mod.size, o_move % board_mod.size))
		board_mod.do_move(o_move)
		end, winner = board_mod.game_end()
		board_gui.title('Your turn.')
		is_opponent_turn = False
		if end:
			messagebox.showinfo('Notification', 'You lose.')
			board_gui.title('You lose.')
			end_game()
	t = threading.Thread(target=opponent_move)
	t.start()

def put_piece(position):
	x, y = position
	gui_cells[x][y].put_piece()

def end_game():
	for cs in gui_cells:
		for c in cs:
			c.mami.unbind(mouse_click)
	print('Game End.')

def run():
	size, n = 6, 4
	board = Board(size=size, n_in_row=n)
	best_policy = PolicyValueNet(size, 'models/3000+/current_policy.model')
	# Below set larger n_playout for better performance
	mcts_player = MCTSPlayer(best_policy.policy_value_fn, c_puct=5, n_playout=400)
	board.init_board((0 if human_first else 1))
	p1, p2 = board.players
	mcts_player.set_player_ind(p2)
	players = {p1: 'Human Player', p2: mcts_player}
	
	app_tk = tk.Tk()
	app_tk.resizable(False, False)
	app_tk.geometry('{}x{}+{}+{}'.format(
		cell_size * size, cell_size * size, cell_size, cell_size
	))
	app_tk.title('Human VS AI - Gomoku')
	for x in range(size):
		cells_column = []
		gui_cells.append(cells_column)
		for y in range(size):
			cells_column.append(GuiCell(app_tk, board, players, (x, y)))

	if False == human_first:
		opponent_turn(app_tk, players, board)
	app_tk.mainloop()

if __name__ == '__main__':
	parser = argparse.ArgumentParser()
	parser.add_argument(
		'-f', '--human_first', action='store_true',
		help='Determine human go first or not.'
	)
	flags = parser.parse_args()
	human_first = flags.human_first
	run()
