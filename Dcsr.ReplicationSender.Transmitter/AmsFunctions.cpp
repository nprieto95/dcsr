#include <string>
#include <ams.h>

#include "StringFunctions.h"

using namespace std;

int register_module(AmsModule* amsModule)
{
	// Values & variables required for registration.
	string
		mibSource = "/home/nahuel/Dcsr/ION/Earth/mib.amsrc",
		applicationName = "dcsr",
		authorityName = "uade",
		unitName = "",
		roleName = "transmitter";

	// Registration
	return ams_register(
		to_c_string(mibSource),
		NULL,
		to_c_string(applicationName),
		to_c_string(authorityName),
		to_c_string(unitName),
		to_c_string(roleName),
		amsModule
	);
}