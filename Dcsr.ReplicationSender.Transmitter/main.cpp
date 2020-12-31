#include <ams.h>

constexpr auto MARS_CONTINUUM = 2;
constexpr auto DELTA_SUBJECT = 2;

int main()
{
	AmsModule module;
	char mibSource[] = "/home/nahuel/Dcsr/ION/Earth/mib.amsrc";
	char applicationName[] = "dcsr";
	char authorityName[] = "uade";
	char unitName[] = "";
	char roleName[] = "transmitter";
	int registrationFailed = ams_register(mibSource, NULL, applicationName, authorityName, unitName, roleName, &module);
	if (!registrationFailed)
		writeMemo("Transmitter successfully registered.");
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


	char message[] = "hello from the pale blue dot";

	/*while (!ams_get_event(module, -1, &currentEvent))
	{
		switch (ams_get_event_type(currentEvent))
		{
		case NOTICE_EVT:
			ams_parse_notice(currentEvent, &stateType, &changeType, &unitNbr, &moduleNbr, &roleNbr, &domainContinuumNbr, &domainUnitNbr, &subjectNbr, &priority, &flowLabel, &sequence, &diligence);
			if (stateType == AmsInvitationState && changeType == AmsStateBegins)
				ams_send(module, MARS_CONTINUUM, 0, moduleNbr, subjectNbr, priority, flowLabel, 28, message, 0);
			break;
		default:
			break;
		}
	}*/

	while (true)
	{
		int publishingFailed = ams_publish(module, DELTA_SUBJECT, 1, 0, 28, message, 0);
		if (publishingFailed)
			break;
		//sleep(30);
		/*int sendingFailed = ams_send(module, MARS_CONTINUUM, 0, 1, DELTA_SUBJECT, 1, 0, 28, message, 0);
		if (sendingFailed)
			break;
		sleep(30);*/
	}

	int unregistrationFailed = ams_unregister(module);
	if (!unregistrationFailed)
		writeMemo("Transmitter successfully unregistered.");
	else
		return -1;
	return 0;
}