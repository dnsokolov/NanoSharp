#!/bin/bash

dotnetversion=`dotnet --info | grep Version: | head -1 | sed -e 's/^\s*//' -e '/^$/d' | grep -E -o [0-9].[0-9] | head -1`
projpath=`pwd`

if [ "$1" == "debug" ]; then

   echo "Debug building..."
   cd Native/MCEngine
   make -f makefile-linux

   if [ $? -gt 0  ]
   then
     exit 1
   fi

   cd $projpath
   dotnet build --configuration Debug
   cp -u Native/MCEngine/mcengine.so bin/Debug/netcoreapp$dotnetversion/mcengine.so

   echo "Enter dotnet run [parameters]"

else

   echo "Realease building..."
   cd Native/MCEngine
   make -f makefile-linux

   if [ $? -gt 0  ]; then
      exit 1
   fi

   cd $projpath
   dotnet build --configuration Release
   cp Native/MCEngine/mcengine.so bin/Release/netcoreapp$dotnetversion/mcengine.so

   rm -r NanoSharpLinux
   mkdir NanoSharpLinux
   cp -rp bin/Release/netcoreapp$dotnetversion/* NanoSharpLinux/
   cp nsc NanoSharpLinux/nsc

   echo "Result saved to NanoSharpLinux"

fi