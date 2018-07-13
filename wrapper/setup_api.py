'''Trying ctypes first'''
import ctypes

def print_all_ctypes():
	types = dir(ctypes)
	print('All things contained in ctypes:')
	for t in types:
		if t.startswith('_'):
			continue
		else:
			print(' ', t)

def enum_device_info():
	'''
	WINSETUPAPI BOOL SetupDiEnumDeviceInfo(
		HDEVINFO         DeviceInfoSet,
		DWORD            MemberIndex,
		PSP_DEVINFO_DATA DeviceInfoData
	);
	'''
	print('==== In enum device info')