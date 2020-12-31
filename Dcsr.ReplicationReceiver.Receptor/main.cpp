#include <ams.h>

constexpr auto EARTH_CONTINUUM = 1;
constexpr auto DELTA_SUBJECT = 2;

int main()
{
	AmsModule module;
	char mibSource[] = "/home/nahuel/Dcsr/ION/Mars/mib.amsrc";
	char applicationName[] = "dcsr";
	char authorityName[] = "uade";
	char unitName[] = "";
	char roleName[] = "receptor";
	int registrationFailed = ams_register(mibSource, NULL, applicationName, authorityName, unitName, roleName, &module);
	if (!registrationFailed)
		writeMemo("Receptor successfully registered.");
	else
		return -1;

	/*int invitationFailed = ams_invite(module, ANY_ROLE, EARTH_CONTINUUM, 0, DELTA_SUBJECT, 1, 0, AmsTransmissionOrder, AmsAssured);
	if (!invitationFailed)
		writeMemo("Receptor successfully invited.");
	else
		return -1;*/

	int subscriptionFailed = ams_subscribe(module, ANY_ROLE, ALL_CONTINUA, 0, DELTA_SUBJECT, 1, 0, AmsTransmissionOrder, AmsAssured);
	if (!subscriptionFailed)
		writeMemo("Receptor successfully subscribed.");
	else
		return -1;

	AmsEvent currentEvent;

	int continuumNbr, unitNbr, moduleNbr, subjectNbr, contentLength, context, priority, roleNbr, domainContinuumNbr, domainUnitNbr;
	unsigned char flowLabel;
	char* content;
	AmsMsgType msgType;
	AmsStateType stateType;
	AmsChangeType changeType;
	AmsSequence sequence;
	AmsDiligence diligence;

	while (!ams_get_event(module, -1, &currentEvent))
	{
		switch (ams_get_event_type(currentEvent))
		{
		case AMS_MSG_EVT:
			ams_parse_msg(currentEvent, &continuumNbr, &unitNbr, &moduleNbr, &subjectNbr, &contentLength, &content, &context, &msgType, &priority, &flowLabel);
			break;
		case NOTICE_EVT:
			ams_parse_notice(currentEvent, &stateType, &changeType, &unitNbr, &moduleNbr, &roleNbr, &domainContinuumNbr, &domainUnitNbr, &subjectNbr, &priority, &flowLabel, &sequence, &diligence);
			break;
		case USER_DEFINED_EVT:
			break;
		default:
			break;
		}
	}

	int unregistrationFailed = ams_unregister(module);
	if (!unregistrationFailed)
		writeMemo("Transmitter successfully unregistered.");
	else
		return -1;
	return 0;
}