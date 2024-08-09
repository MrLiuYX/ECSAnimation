set WORKSPACE=..

set GEN_CLIENT=%WORKSPACE%\luban\Luban.ClientServer\Luban.ClientServer.exe
set CONF_ROOT=%WORKSPACE%\luban\Config

%GEN_CLIENT% -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %WORKSPACE%\Assets\Scripts\LCFramework\LubanGenConfigCode ^
 --output_data_dir %WORKSPACE%\Assets\AssetBundleRes\Main\Config ^
 --gen_types code_cs_unity_json,data_json ^
 -s all 

pause