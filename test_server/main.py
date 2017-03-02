from bottle import route, run, post
import google.protobuf
import hello_pb2

@post('/')
def hello():
    r = hello_pb2.RHello()
    r.r1 = 11
    r.r2 = 22
    print(r)
    return r.SerializeToString()

run(host='localhost', port=8080, debug=True)
