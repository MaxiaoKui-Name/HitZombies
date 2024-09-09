set WORKSPACE=.
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\MiniTemplate

dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-simple-json^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x inputDataDir=%CONF_ROOT%\Datas ^
    -x outputCodeDir=..\Assets\Scripts\Luban^
    -x outputDataDir=..\Assets\LubanConfigs
  
pause
 
