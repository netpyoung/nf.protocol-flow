# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: world.proto

import sys
_b=sys.version_info[0]<3 and (lambda x:x) or (lambda x:x.encode('latin1'))
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
from google.protobuf import descriptor_pb2
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='world.proto',
  package='protocol',
  syntax='proto3',
  serialized_pb=_b('\n\x0bworld.proto\x12\x08protocol\" \n\x06QWorld\x12\n\n\x02q1\x18\x01 \x01(\x05\x12\n\n\x02q2\x18\x02 \x01(\x05\" \n\x06RWorld\x12\n\n\x02r1\x18\x01 \x01(\x05\x12\n\n\x02r2\x18\x02 \x01(\x05\x42\x18\xaa\x02\x15\x41utoGenerated.Messageb\x06proto3')
)




_QWORLD = _descriptor.Descriptor(
  name='QWorld',
  full_name='protocol.QWorld',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='q1', full_name='protocol.QWorld.q1', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='q2', full_name='protocol.QWorld.q2', index=1,
      number=2, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=25,
  serialized_end=57,
)


_RWORLD = _descriptor.Descriptor(
  name='RWorld',
  full_name='protocol.RWorld',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='r1', full_name='protocol.RWorld.r1', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='r2', full_name='protocol.RWorld.r2', index=1,
      number=2, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=59,
  serialized_end=91,
)

DESCRIPTOR.message_types_by_name['QWorld'] = _QWORLD
DESCRIPTOR.message_types_by_name['RWorld'] = _RWORLD
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

QWorld = _reflection.GeneratedProtocolMessageType('QWorld', (_message.Message,), dict(
  DESCRIPTOR = _QWORLD,
  __module__ = 'world_pb2'
  # @@protoc_insertion_point(class_scope:protocol.QWorld)
  ))
_sym_db.RegisterMessage(QWorld)

RWorld = _reflection.GeneratedProtocolMessageType('RWorld', (_message.Message,), dict(
  DESCRIPTOR = _RWORLD,
  __module__ = 'world_pb2'
  # @@protoc_insertion_point(class_scope:protocol.RWorld)
  ))
_sym_db.RegisterMessage(RWorld)


DESCRIPTOR.has_options = True
DESCRIPTOR._options = _descriptor._ParseOptions(descriptor_pb2.FileOptions(), _b('\252\002\025AutoGenerated.Message'))
# @@protoc_insertion_point(module_scope)
