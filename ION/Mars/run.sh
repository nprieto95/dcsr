ionstop
rm ion.log
ionstart -I ion.rc
set +e
sudo tc qdisc del dev eth0 root
set -e
sudo tc qdisc add dev eth0 root netem delay 274877ms
