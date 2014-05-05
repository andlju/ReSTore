#!/bin/sh 

apt-get install mono-runtime

wget http://download.geteventstore.com/binaries/eventstore-mono-v3.0.0rc2.tar.gz

tar -xvzf eventstore-mono-v3.0.0rc2.tar.gz


if [ ! -f "eventstore-mono-v3.0.0rc2/singlenode-config.json" ]; then
  echo "file missing"
  cat <<EOF > eventstore-mono-v3.0.0rc2/singlenode-config.json
{ ip: "192.168.10.2" }
EOF
fi

cd eventstore-mono-v3.0.0rc2/

./run-singlenode.sh &