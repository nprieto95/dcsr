ionstop
rm ion.log
set +e
rm *.petition.log
sudo tc qdisc del dev eth0 root
set -e
#sudo tc qdisc add dev eth0 root handle 1: prio
#sudo tc qdisc add dev eth0 parent 1:3 handle 30: tbf rate 20kbit buffer 1600 limit 3000
#sudo tc qdisc add dev eth0 parent 30:1 handle 31: netem delay 274877ms
#sudo tc filter add dev eth0 protocol ip parent 1:0 prio 3 u32 match ip dst 192.168.0.88 flowid 1:3
ionstart -I ion.rc
amsd mib.amsrc @ dcsr uade "" &
sleep 90
ramsgate dcsr uade &
