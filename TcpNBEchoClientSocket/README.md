My Project
This project was written to solve this problem. I am an msp contracted with unitreds, a kaseya company.
I have my business network and my clients networks backed up on my on-premises unitrends appliance. However my
copy backups to the unitrends cloud are failing. I need to have write access to destination 64.124.171.193 on udp port 4385.
i get the error message: checking 69.124.171.193 via port 4385/udp for openvpn server   Failed.
so i wrote this tcp/ip sockets api program to help analuse this situation. i am currently tied up in a array out of range error.
i am hoping collaboration will help resolve this issue.
if you createa visual studio project out of this repository, right click the project node and select Properties.
The command line arguments will be: 64.124.171.193 "Help" 4385