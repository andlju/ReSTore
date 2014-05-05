#!/bin/sh 

mkdir -p /etc/puppet/modules

(puppet module list | grep puppetlabs-rabbitmq) ||
   puppet module install puppetlabs/rabbitmq
