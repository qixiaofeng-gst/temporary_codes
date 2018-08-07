# -*- coding: utf-8 -*-
"""
@author: Junxiao Song
"""

from __future__ import print_function
import numpy as np

invalid_player = -1

class Board(object):
	"""board for the game"""

	def __init__(self, **kwargs):
		self.size = int(kwargs.get('size', 8))
		# board states stored as a dict,
		# key: move as location on the board,
		# value: player as pieces type
		self.states = {}
		# need how many pieces in a row to win
		self.n_in_row = int(kwargs.get('n_in_row', 5))
		self.players = [1, 2]  # player1 and player2
		self.current_player = 1
		self._winner = invalid_player
	
	def _check_if_win(self, move):
		min_count = 2 * self.n_in_row - 1
		if min_count > len(self.states):
			return
		
		checking_player = self.states.get(move, invalid_player)
		if invalid_player == checking_player:
			return
		
		x0 = move % self.size
		y0 = move // self.size
		xs = np.full(self.size, x0)
		ys = np.full(self.size, y0)
		left = range(x0 - 1, -1, -1)
		right = range(x0, self.size)
		up = range(y0 - 1, -1, -1)
		down = range(y0, self.size)
		
		def count_(arr_x, arr_y):
			count = 0
			for x, y in zip(arr_x, arr_y):
				if checking_player == self.states.get(y * self.size + x, invalid_player):
					count += 1
				else:
					return count
			return count
	
		if (
			self.n_in_row == (count_(left, ys) + count_(right, ys)) or
			self.n_in_row == (count_(xs, up) + count_(xs, down)) or
			self.n_in_row == (count_(left, down) + count_(right, up)) or
			self.n_in_row == (count_(left, up) + count_(right, down))
		):
			self._winner = checking_player

	def init_board(self, start_player=0):
		if self.size < self.n_in_row:
			raise Exception('board size can not be less than {}'.format(self.n_in_row))
		self.current_player = self.players[start_player]  # start player
		# keep available moves in a list
		self.availables = list(range(self.size * self.size))
		self.states = {}
		self.last_move = -1

	def location_to_move(self, location):
		if len(location) != 2:
			return -1
		h = location[0]
		w = location[1]
		move = h * self.size + w
		if move not in range(self.size * self.size):
			return -1
		return move

	def current_state(self):
		"""return the board state from the perspective of the current player.
		state shape: 4*size*size
		"""

		square_state = np.zeros((4, self.size, self.size))
		if self.states:
			moves, players = np.array(list(zip(*self.states.items())))
			move_curr = moves[players == self.current_player]
			move_oppo = moves[players != self.current_player]
			square_state[0][move_curr // self.size, move_curr % self.size] = 1.0
			square_state[1][move_oppo // self.size, move_oppo % self.size] = 1.0
			# indicate the last move location
			square_state[2][self.last_move // self.size, self.last_move % self.size] = 1.0
		if len(self.states) % 2 == 0:
			square_state[3][:, :] = 1.0  # indicate the colour to play
		return square_state[:, ::-1, :]

	def do_move(self, move):
		self.states[move] = self.current_player
		self._check_if_win(move)
		self.availables.remove(move)
		self.current_player = (
			self.players[0] if self.current_player == self.players[1]
			else self.players[1]
		)
		self.last_move = move

	def has_a_winner(self):
		size = self.size
		states = self.states
		n = self.n_in_row

		moved = list(set(range(size * size)) - set(self.availables))
		if len(moved) < self.n_in_row + 2:
				return False, -1

		for m in moved:
			h = m // size
			w = m % size
			player = states[m]

			if (w in range(size - n + 1) and len(set(states.get(i, -1) for i in range(m, m + n))) == 1):
				return True, player

			if (h in range(size - n + 1) and len(set(states.get(i, -1) for i in range(m, m + n * size, size))) == 1):
				return True, player

			if (w in range(size - n + 1) and h in range(size - n + 1) and len(set(states.get(i, -1) for i in range(m, m + n * (size + 1), size + 1))) == 1):
				return True, player

			if (w in range(n - 1, size) and h in range(size - n + 1) and len(set(states.get(i, -1) for i in range(m, m + n * (size - 1), size - 1))) == 1):
				return True, player

		return False, -1

	def game_end(self):
		"""Check whether the game is ended or not"""
		win, winner = self.has_a_winner()
		if win:
			return True, winner
		elif 0 == len(self.availables):
			return True, -1
		return False, -1

	def get_current_player(self):
		return self.current_player

class Game(object):
	"""game server"""

	def __init__(self, board, **kwargs):
		self.board = board

	def graphic(self, board, player1, player2):
		"""Draw the board and show game info"""
		size = board.size

		print("Player", player1, "with X".rjust(3))
		print("Player", player2, "with O".rjust(3))
		print()
		for x in range(size):
			print("{0:8}".format(x), end='')
		print('\r\n')
		for i in range(size - 1, -1, -1):
			print("{0:4d}".format(i), end='')
			for j in range(size):
				loc = i * size + j
				p = board.states.get(loc, -1)
				if p == player1:
					print('X'.center(8), end='')
				elif p == player2:
					print('O'.center(8), end='')
				else:
					print('_'.center(8), end='')
			print('\r\n\r\n')

	def start_self_play(self, player, is_shown=0, temp=1e-3):
		""" start a self-play game using a MCTS player, reuse the search tree,
		and store the self-play data: (state, mcts_probs, z) for training
		"""
		self.board.init_board()
		p1, p2 = self.board.players
		states, mcts_probs, current_players = [], [], []
		while True:
			move, move_probs = player.get_action(
				self.board, temp=temp, return_prob=1
			)
			# store the data
			states.append(self.board.current_state())
			mcts_probs.append(move_probs)
			current_players.append(self.board.current_player)
			# perform a move
			self.board.do_move(move)
			if is_shown:
				self.graphic(self.board, p1, p2)
			end, winner = self.board.game_end()
			if end:
				# winner from the perspective of the current player of each state
				winners_z = np.zeros(len(current_players))
				if winner != -1:
					winners_z[np.array(current_players) == winner] = 1.0
					winners_z[np.array(current_players) != winner] = -1.0
				# reset MCTS root node
				player.reset_player() # Must reset, or the trained AI will be very stupid.
				if is_shown:
					if winner != -1:
						print("Game end. Winner is player:", winner)
					else:
						print("Game end. Tie")
				return winner, zip(states, mcts_probs, winners_z)
'''
Notes:
If the MC tree is not cleaned every single self play,
	the training will be stuck in several scenario,
	will cause the trained AI stupid.
So should better use reset_player() to clean the MC tree.
'''
