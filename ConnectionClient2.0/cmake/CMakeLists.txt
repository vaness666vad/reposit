cmake_minimum_required(VERSION 2.6)

project(nathole)
message("main path ${CMAKE_SOURCE_DIR}" )
set(CMAKE_MODULE_PATH ${CMAKE_MODULE_PATH} "${CMAKE_SOURCE_DIR}")
set(VERSION 2.0)

set(EXE_NAME client.exe)

find_package( Threads REQUIRED )
message("CMAKE_THREAD_LIBS_INIT : " ${CMAKE_THREAD_LIBS_INIT})

include(FindV4L2)
find_package(V4L2 REQUIRED)
include_directories(${V4L2_INCLUDE_DIR})


include(FindOpenSSL)
find_package(OpenSSL REQUIRED)
include_directories(${OPENSSL_INCLUDE_DIR})

add_executable(${EXE_NAME} main.cpp)

target_link_libraries(${EXE_NAME} ${OPENSSL_LIBRARIES} ${V4L2_LIBRARIES})
target_link_libraries(${EXE_NAME} ${CMAKE_THREAD_LIBS_INIT})

