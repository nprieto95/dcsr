#include <iostream>
#include <string>
#include <algorithm>

#include <boost/json.hpp>
using boost::json::parse;
using boost::json::value;

#include <cfdp.h>

#include "storage_functions.h"
#include "change.h"

using namespace std;

int main()
{

    auto queue = get_queue();
    auto attach_result = cfdp_attach();
    CfdpNumber mars_cfdp_nbr;
    CfdpTransactionId transaction_id;
    cfdp_compress_number(&mars_cfdp_nbr, 2);
    auto requests = cfdp_create_fsreq_list();
    cfdp_add_fsreq(requests, CfdpAction::CfdpDenyFile, "/home/nahuel/Documents/test_transfer_delay1515.odt", NULL);
    cfdp_add_fsreq(requests, CfdpAction::CfdpRenameFile, "/home/nahuel/Documents/test_transfer_delay1515_new.odt", "/home/nahuel/Documents/test_transfer_delay1515.odt");
    auto put_result = cfdp_put(
        &mars_cfdp_nbr,
        0,
        NULL,
        "/home/nahuel/Documents/test_transfer_delay.odt",
        "/home/nahuel/Documents/test_transfer_delay1515_new.odt",
        NULL, NULL, NULL, 0, NULL, 0, NULL, requests, &transaction_id);

    /*cfdp_add_fsreq(requests, CfdpAction::CfdpDeleteFile, "/home/nahuel/Documents/test_transfer_delay.odt", NULL);
    auto delete_result = cfdp_put(
        &mars_cfdp_nbr,
        0,
        NULL,
        NULL,
        NULL,
        NULL, NULL, NULL, 0, NULL, 0, NULL, requests, &transaction_id);*/
    cfdp_detach();

    do
    {
        
        queue.download_attributes();

        auto approximate_message_count = queue.approximate_message_count();

        if (approximate_message_count == 0)
        {
            continue;
        }

        auto messages = queue.get_messages(approximate_message_count);

        for (auto message : messages)
        {
            auto message_string = message.content_as_string();
            value jv = parse(message_string);
            if (jv.is_array())
            {
                auto a = jv.as_array();
                auto o = a[0].as_object();
                auto size = o["size"].as_int64();
                cout << "Size is " << size;
            }
        }

    } while (!sleep(10));

    return 0;

}