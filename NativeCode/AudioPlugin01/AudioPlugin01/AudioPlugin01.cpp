// AudioPlugin01.cpp : Defines the exported functions for the DLL application.
//

#include "header.h"

extern "C" {
	_declspec(dllexport) double Mult(float a, float b) {
		return a * b;
	}
}


