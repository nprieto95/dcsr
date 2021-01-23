#include <iostream>
#include <string>

using namespace std;

string get_message(string &message, string prompt)
{
    cout << prompt << endl;
    getline(cin, message); 
    return message;
}