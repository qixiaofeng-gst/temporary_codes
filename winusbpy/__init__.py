import os
if os.name == 'nt':
	from winusbpy.winusbpy import *
	from winusbpy.winusb import *
else:
	raise ImportError("WinUsbPy only works on Windows platform")
