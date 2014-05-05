class { '::rabbitmq':
  delete_guest_user => false
}

rabbitmq_user { 'anders':
  admin    => true,
  password => 'test',
  provider => 'rabbitmqctl',
}
