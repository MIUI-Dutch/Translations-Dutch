:: 
:: xavierjohn22's AUTO APKTOOL
:: File: AutoAPKToolMenu.bat
:: Description: Tool to automate APK edits
::
@echo off
java -version 
if errorlevel 1 goto errjava
cls
cls
COLOR 0A
::mode con:cols=115 lines=45
setLocal EnableDelayedExpansion
set logMain=log_main.txt
if (%1)==(0) goto skipme
echo -------------------------------------------------------------- >> %logMain%
echo ^%date% -- %time%^ >> %logMain%
echo -------------------------------------------------------------- >> %logMain%
AutoAPKToolMain 0 2>> %logMain%
:skipme
::set apktool=apktool_1.3.2.jar
set foundversion=0
set foundcopydistapk=0
set valuecopydistapk=MALI
set foundgenzipaligncomp=0
set valuegenzipaligncomp=MALI
set adbsdroot=/sdcard/
set adbsdfolder=xRoms
set overwrite_arsc=1
set overwrite_dex=1
set overwrite_custom=1
set copy_other_res=1
set forcedelstart=1
set forcedelend=1
set optimizepng=0
set optimizeonly9png=0
::set complevel=9
set foundcomplevel=0
set genzipalign=1
set zipalignthensign=1
::set copydistapk=0
::set genzipaligncomp=0
set logR=log_recompile.txt
set logD=log_decompile.txt
if not exist "_ADB" mkdir "_ADB"
if not exist "_MODS" mkdir "_MODS"
if not exist "_INPUT_APK" mkdir "_INPUT_APK"
if not exist "_OUT_APK" mkdir "_OUT_APK"
if not exist "_OUT_APKTOOL_DIST" mkdir "_OUT_APKTOOL_DIST"
if not exist "_CUSTOM_RES" mkdir "_CUSTOM_RES"
if not exist "_OUT_APK_ZIPALIGN" mkdir "_OUT_APK_ZIPALIGN"
if not exist "_OUT_APKTOOL_DIST_ZIPALIGN" mkdir "_OUT_APKTOOL_DIST_ZIPALIGN"
if not exist "_THEMES" mkdir "_THEMES"
if not exist "_OUTPUT_ROM" mkdir "_OUTPUT_ROM"
if not exist "_INPUT_ROM" mkdir "_INPUT_ROM"
if not exist "_TEMP" mkdir "_TEMP"
if not exist "_TRANSLATE" mkdir "_TRANSLATE"
if not exist "Settings" mkdir "Settings"
if not exist "Settings/AutoAPKToolMain.ini" goto :oneiniisnotthere
for /f "tokens=1,2 delims==" %%a in (Settings/AutoAPKToolMain.ini) do ( 
if %%a==apktoolversion if exist %%b (
set apktool=%%b
set foundversion=1
)
)
:oneiniisnotthere
if %foundversion%==0 (set apktool=apktool_1.3.2.jar)
if not exist "Settings/AutoAPKToolCopyOutSettings.ini" goto :twoiniisnotthere
for /f "tokens=1,2 delims==" %%a in (Settings/AutoAPKToolCopyOutSettings.ini) do ( 
if %%a==copydistapksettings (
set valuecopydistapk=%%b
set foundcopydistapk=1
)
)
if %valuecopydistapk%==TAMA (set copydistapk=1)
if %valuecopydistapk%==MALI (set copydistapk=0)
:twoiniisnotthere
if %foundcopydistapk%==0 (set copydistapk=0)
if not exist "Settings/AutoAPKToolGenZipAlSettings.ini" goto :threeiniisnotthere
for /f "tokens=1,2 delims==" %%a in (Settings/AutoAPKToolGenZipAlSettings.ini) do ( 
if %%a==genzipalignedsettings (
set valuegenzipaligncomp=%%b
set foundgenzipaligncomp=1
)
)
if %valuegenzipaligncomp%==TAMA (set genzipaligncomp=1)
if %valuegenzipaligncomp%==MALI (set genzipaligncomp=0)
:threeiniisnotthere
if %foundgenzipaligncomp%==0 (set genzipaligncomp=0)
if not exist "Settings/AutoAPKToolCompLevelSettings.ini" goto :fouriniisnotthere
for /f "tokens=1,2 delims==[" %%a in (Settings/AutoAPKToolCompLevelSettings.ini) do ( 
if %%a==complevelsettings (
set complevel=%%b
set foundcomplevel=1
)
)
:fouriniisnotthere
if %foundcomplevel%==0 (set complevel=9)
:restart
set adbsdfolderpath=%adbsdroot%%adbsdfolder%
if %apktool%==apktool_1.3.1.jar (set apktoolversion=1.3.1)
if %apktool%==apktool_1.3.2.jar (set apktoolversion=1.3.2)
if %apktool%==apktool_1.4.1.jar (set apktoolversion=1.4.1)
if %overwrite_arsc%==0 (set tmparsc32905=OFF) else (set tmparsc32905=ON)
if %overwrite_dex%==0 (set tmpdex526786=OFF) else (set tmpdex526786=ON)
if %overwrite_custom%==0 (set tmpcustomres=OFF) else (set tmpcustomres=ON)
if %copy_other_res%==0 (set tmpcopyres24=OFF) else (set tmpcopyres24=ON)
if %forcedelstart%==0 (set tmpfdelstart=OFF) else (set tmpfdelstart=ON)
if %forcedelend%==0 (set tmpfdelend23=OFF) else (set tmpfdelend23=ON)
if %optimizepng%==0 (set tmpoptimizep=OFF) else (set tmpoptimizep=ON)
if %optimizeonly9png%==0 (set tmpoptim9png=OFF) else (set tmpoptim9png=ON)
if %genzipalign%==0 (set tmpgenzipali=OFF) else (set tmpgenzipali=ON)
if %copydistapk%==0 (set tmpcopydistapk=OFF) else (set tmpcopydistapk=ON)
if %genzipaligncomp%==0 (set tmpgenzipalicomp=OFF) else (set tmpgenzipalicomp=ON)
if %zipalignthensign%==0 (set tmpzipsigned=OFF) else (set tmpzipsigned=ON)
echo -----------------------------------------------------------------------------------------------------------------
echo - AUTO APKTOOL V2.0.3        : Current directory is %~dp0
echo - by xavierjohn22            : Comression level set to (%complevel%)
echo -----------------------------------------------------------------------------------------------------------------
echo USING APK TOOL VERSION       : %apktoolversion% : change version using menu (21)
echo adb sdcard folder set to     : %adbsdfolderpath% : set using menu (46)
echo -----------------------------------------------------------------------------------------------------------------
echo BASICS:
echo    1   if framework-res.apk       6   Single : Decompile APK (File)       11  Copy_Resources.txt to _CUSTOM_RES
echo    2   All : Decompile APKs       7   Single : Recompile APK (Folder)     12  Zipaligned APKS in _OUT_APK
echo    3   All : Recompile APKs       8   Single : Build Usable APK           13  Move _OUT_APK_ZIPALIGN to _OUT_APK
echo    4   All : Build Usable APKs    9   Single : Recompile-Build (Folder)
echo    5   Build a Flashable ZIP      10  Sign Update ZIPs / APKs		 
echo SETTINGS:
echo    21  Change apktool version
echo    22  Set compression level for optimization
echo    23  Overwrite resources.arsc                   (%tmparsc32905%)	28  Force delete dir on Decompile (-f)     (%tmpfdelstart%)
echo    24  Overwrite classes.dex                      (%tmpdex526786%)	29  Force delete dir on Recompile (-f)     (%tmpfdelend23%)
echo    25  Overwrite using _CUSTOM_RES                (%tmpcustomres%)	30  Optimized All PNGS in APKs             (%tmpoptimizep%)
echo    26  Copy added resources to APK                (%tmpcopyres24%)	31  Optimized "xyz.9.png" PNGS in APKs     (%tmpoptim9png%)
echo    27  Generate zipaligned apk from _OUT_APK      (%tmpgenzipali%)
echo SETTINGS FOR APKTOOL DIST GENERATED APKs:
echo    42  Copy recompiled apk to _OUT_APKTOOL_DIST   (%tmpcopydistapk%)
echo    43  Generate zipaligned apk after recompile    (%tmpgenzipalicomp%)
echo TOOLS AND INFO:
echo    50  Clear All APKs in _INPUT_APK folder        53  Clear All Logs
echo    51  Clear All APKs in _OUT...... folders       54  View Logs
echo    52  Clear All APKs in _FLASHABLES folder 
echo ADVANCED:
echo    60  adb devices                                66  Set adb "sdcard folder"
echo    61  adb install                                67  Pull all files in "sdcard folder"
echo    62  adb push to /system/app                    68  Push a file to "sdcard folder"
echo    63  adb pull /system/app                       69  adb reboot
echo    64  adb pull /data/app
echo    65  adb pull /framework
echo SPECIALS:
echo    80  Apply Dev Mod Files                        83  Apply Hue and Saturation on pngs in folder
echo    81  Install additional framework file          84  Build themed apks (png folders in _THEMES)
echo    82  Create theme folders / Mogrify images      85  Sign APKs after build themed apks (%tmpzipsigned%)
echo TRANSLATIONS:
echo   001  Copy translation folder(s) in _INPUT_APK folder
echo   002  Delete unnecessary folder(s) in _INPUT_APK folder
echo   003  Automatic                                 004  Automatic without precompile
echo   005  Make update zip (put *.apk in _UPDATE_APK folder)
echo HELP:
echo    98  Display How-To                             99  Exit
echo -----------------------------------------------------------------------------------------------------------------
echo.
SET menunr=
SET /P menunr=PLEASE CHOOSE TASK: 
if '%menunr%'=='' goto :restart
IF %menunr%==001 (goto installtranslation)
IF %menunr%==002 (goto removeunnes)
IF %menunr%==003 (goto automatic)
IF %menunr%==004 (goto automaticnoprecompile)
IF %menunr%==005 (goto makeupdatezip)
IF %menunr%==1 (goto ifframework)
IF %menunr%==2 (goto decompile)
IF %menunr%==3 (goto recompile)
IF %menunr%==4 (goto rebuildusable)
IF %menunr%==5 (goto buildflashable)
IF %menunr%==6 (goto singledecompile)
IF %menunr%==7 (goto singlerecompile)
IF %menunr%==8 (goto singlebuildusableapk)
IF %menunr%==9 (goto singlerecthenbuild)
IF %menunr%==10 (goto signzip)
IF %menunr%==11 (goto copyresourcestxt)
IF %menunr%==12 (goto zipalignoutapks)
IF %menunr%==13 (goto movezipaligntoout)
IF %menunr%==21 (goto setapktoolversion)
IF %menunr%==22 (goto setcomplevel)
IF %menunr%==23 (goto setarsc)
IF %menunr%==24 (goto setdex)
IF %menunr%==25 (goto setovercustom)
IF %menunr%==26 (goto setcopyres)
IF %menunr%==27 (goto generatezipalign)
IF %menunr%==28 (goto setforcedelstart)
IF %menunr%==29 (goto setforcedelend)
IF %menunr%==30 (goto optipng)
IF %menunr%==31 (goto optionly9png)
IF %menunr%==42 (goto copydistapktoout)
IF %menunr%==43 (goto generatezipaligncomp)
IF %menunr%==50 (goto cleaninput)
IF %menunr%==51 (goto cleanout)
IF %menunr%==52 (goto cleanflashable)
IF %menunr%==53 (goto clearlogs)
IF %menunr%==54 (goto viewlog)
IF %menunr%==60 (goto adbdevices)
IF %menunr%==61 (goto adbinstall)
IF %menunr%==62 (goto adbpushsystemapp)
IF %menunr%==63 (goto adbpullsystemapp)
IF %menunr%==64 (goto adbpulldataapp)
IF %menunr%==65 (goto adbpullframework)
IF %menunr%==66 (goto setadbsdfolder)
IF %menunr%==67 (goto pullfromadbsdfolder)
IF %menunr%==68 (goto pushfileadbsdfolder)
IF %menunr%==69 (goto adbreboot)
IF %menunr%==80 (goto devmods)
IF %menunr%==81 (goto installaddapk)
IF %menunr%==82 (goto createthemefolders)
IF %menunr%==83 (goto applyhuesaturation)
IF %menunr%==84 (goto buildthemedapks)
IF %menunr%==85 (goto zipalignandsign)
IF %menunr%==98 (goto help)
IF %menunr%==99 (goto quit)
:nope
echo ERROR: Not part of the MENU
PAUSE
goto restart
:setarsc
if %overwrite_arsc%==1 (set overwrite_arsc=0) else (set overwrite_arsc=1)
cls
goto restart
:setdex
if %overwrite_dex%==1 (set overwrite_dex=0) else (set overwrite_dex=1)
cls
goto restart
:setovercustom
if %overwrite_custom%==1 (set overwrite_custom=0) else (set overwrite_custom=1)
cls
goto restart
:setcopyres
if %copy_other_res%==1 (set copy_other_res=0) else (set copy_other_res=1)
cls
goto restart
:setforcedelstart
if %forcedelstart%==1 (set forcedelstart=0) else (set forcedelstart=1)
cls
goto restart
:setforcedelend
if %forcedelend%==1 (set forcedelend=0) else (set forcedelend=1)
cls
goto restart
:optipng
if %optimizepng%==1 (set optimizepng=0) else (set optimizepng=1)
cls
goto restart
:optionly9png
if %optimizeonly9png%==1 (set optimizeonly9png=0) else (set optimizeonly9png=1)
cls
goto restart
:generatezipalign
if %genzipalign%==1 (set genzipalign=0) else (set genzipalign=1)
cls
goto restart
:copydistapktoout
if %copydistapk%==1 (set copydistapk=0) else (set copydistapk=1)
type "Settings/AutoAPKToolCopyOutSettings.ini" | find /v "copydistapksettings=" > "Settings/AutoAPKToolCopyOutSettings.ini"
if %copydistapk%==0 (echo copydistapksettings=MALI>> "Settings/AutoAPKToolCopyOutSettings.ini")
if %copydistapk%==1 (echo copydistapksettings=TAMA>> "Settings/AutoAPKToolCopyOutSettings.ini")
cls
goto restart
:generatezipaligncomp
if %genzipaligncomp%==1 (set genzipaligncomp=0) else (set genzipaligncomp=1)
type "Settings/AutoAPKToolGenZipAlSettings.ini" | find /v "genzipalignedsettings=" > "Settings/AutoAPKToolGenZipAlSettings.ini"
if %genzipaligncomp%==0 (echo genzipalignedsettings=MALI>> "Settings/AutoAPKToolGenZipAlSettings.ini")
if %genzipaligncomp%==1 (echo genzipalignedsettings=TAMA>> "Settings/AutoAPKToolGenZipAlSettings.ini")
cls
goto restart
:: ------------------------------
:createthemefolders
set NUMCNT=
set /P NUMCNT=Enter number of folders to create (will be use as Hue incrementing): %=%
if '%NUMCNT%'=='' goto :createthemefolders
if %NUMCNT% GTR 360 (
echo Invalid Folder Count or Hue Level, choose between 1-360
goto :createthemefolders
)
if %NUMCNT% LSS 1 (
echo Invalid Folder Count or Hue Level, choose between 1-360
goto :createthemefolders
)
:verifysaturation
set SATUR=
set /P SATUR=Enter Saturation Value (will be use for each folder): %=%
if '%SATUR%'=='' goto :verifysaturation
if %SATUR% GTR 360 (
echo Invalid Saturation Level, choose between 1-360
goto :verifysaturation
)
if %SATUR% LSS 1 (
echo Invalid Saturation Level, choose between 1-360
goto :verifysaturation
)
:verifyfolderpath
set HSFORFOLDERONLY=0
set INPUT=
set /P INPUT=Drag FOLDER here: %=%
IF NOT EXIST %INPUT%\NUL goto :verifyfolderpath
set TotI=0
set starttime=%TIME%
FOR /L %%N IN (1,1,%NUMCNT%) DO (
set COUNT=%%N
call 07folders.bat %INPUT% %COUNT% %SATUR%
)
echo Report
echo Folder path     : %INPUT%
echo Start Time      : %starttime%
echo End Time        : %TIME%
echo Folders created : %COUNT%
echo Total files     : %TotI%
pause
cls
goto restart
:: ------------------------------
:applyhuesaturation
set NUMCNT=
set /P NUMCNT=Enter Hue level (1-360): %=%
if '%NUMCNT%'=='' goto :createthemefolders
if %NUMCNT% GTR 360 (
echo Invalid Hue Level, choose between 1-360
goto :applyhuesaturation
)
if %NUMCNT% LSS 1 (
echo Invalid Hue Level, choose between 1-360
goto :applyhuesaturation
)
:applysaturation
set SATUR=
set /P SATUR=Enter Saturation level (1-360): %=%
if '%SATUR%'=='' goto :applysaturation
if %SATUR% GTR 360 (
echo Invalid Saturation Level, choose between 1-360
goto :applysaturation
)
if %SATUR% LSS 1 (
echo Invalid Saturation Level, choose between 1-360
goto :applysaturation
)
:validatethisfolder
set INPUT=
set /P INPUT=Drag FOLDER here: %=%
IF NOT EXIST %INPUT%\NUL goto :validatethisfolder
set HSFORFOLDERONLY=1
call 07folders.bat %INPUT% %COUNT% %SATUR%
set HSFORFOLDERONLY=0
pause
cls
goto restart
:: ------------------------------
:buildthemedapks
for /f "tokens=* delims= " %%D in ('dir _THEMES /b/ad') do (
call 08createthemedAPK.bat %%D
)
pause
cls
goto restart
:: ------------------------------
:zipalignandsign
if %zipalignthensign%==1 (set zipalignthensign=0) else (set zipalignthensign=1)
cls
goto restart
:makeupdatezip
@echo off
SET version=
SET /P version=Version (the LAST Official MIUI version. example: 1.10.21): 
if '%version%'=='exit' goto :restart
if '%version%'=='' goto :makeupdatezip
SET forversion=
SET /P forversion=new UPDATE MIUI Version (example: 1.10.26): 
if '%forversion%'=='exit' goto :restart
if '%forversion%'=='' goto :makeupdatezip
SET BPU=
SET /P BPU=build.prop update (empty=No, anything else=Yes, exit=Exit): 
if '%BPU%'=='exit' goto :restart
for /f %%d in ('dir /b /a:d _UPDATE_ROM') do rd /s /q _UPDATE_ROM\%%d
for /f %%f in ('dir /b _UPDATE_ROM') do del /q _UPDATE_ROM\%%f
for /f %%d in ('dir /b /a:d _UPDATE_INPUT') do xcopy /e /y /f /i _UPDATE_INPUT\%%d _UPDATE_ROM\%%d
copy /y _UPDATE_APK\*.* _UPDATE_ROM\system\app

@echo ui_print(""); >_UPDATE_ROM\updater-script
@echo ui_print("************************************************"); >>_UPDATE_ROM\updater-script
@echo ui_print("*                                              *"); >>_UPDATE_ROM\updater-script
@echo ui_print("*   ##      ## ## ##  ## ##    ##   ## ##      *"); >>_UPDATE_ROM\updater-script
@echo ui_print("*   ###    ### ## ##  ## ##    ###  ## ##      *"); >>_UPDATE_ROM\updater-script
@echo ui_print("*   ## #### ## ## ##  ## ##    #### ## ##      *"); >>_UPDATE_ROM\updater-script
@echo ui_print("*   ##  ##  ## ## ##  ## ##    ## #### ##      *"); >>_UPDATE_ROM\updater-script
@echo ui_print("*   ##  ##  ## ## ##  ## ##    ##  ### ##      *"); >>_UPDATE_ROM\updater-script
@echo ui_print("*   ##      ## ##  ####  ##    ##   ## #####   *"); >>_UPDATE_ROM\updater-script
@echo ui_print("*                                              *"); >>_UPDATE_ROM\updater-script
@echo ui_print("************************************************"); >>_UPDATE_ROM\updater-script
@echo ui_print(""); >>_UPDATE_ROM\updater-script
@echo ui_print("Awaiting installation.............."); >>_UPDATE_ROM\updater-script
@echo unmount("/system"); >>_UPDATE_ROM\updater-script
@echo assert(getprop("ro.build.version.incremental") == "%version%"); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "captivatemtd",mount("yaffs2", "MTD", "system", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "umts_jordan",mount("ext3", "EMMC", "/dev/block/mmcblk1p21", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "bravo",mount("yaffs2", "MTD", "system", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "saga",mount("ext3", "EMMC", "/dev/block/mmcblk0p25", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "vision",mount("ext3", "EMMC", "/dev/block/mmcblk0p25", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "ace",mount("ext4", "EMMC", "/dev/block/mmcblk0p25", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "leo",mount("yaffs2", "MTD", "system", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "vivo",mount("ext3", "EMMC", "/dev/block/mmcblk0p25", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "umts_sholes",mount("yaffs2", "MTD", "system", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "passion",mount("yaffs2", "MTD", "system", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "crespo",mount("ext4", "EMMC", "/dev/block/platform/s3c-sdhci.0/by-name/system", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "p990",mount("ext3", "EMMC", "/dev/block/mmcblk0p1", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "galaxys2",mount("ext4", "EMMC", "/dev/block/mmcblk0p9", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "galaxys",mount("yaffs2", "MTD", "system", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "mione_plus",mount("ext4", "EMMC", "/dev/block/platform/msm_sdcc.1/by-name/system", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(getprop("ro.product.device") == "vibrantmtd",mount("yaffs2", "MTD", "system", "/system"),ui_print("##")); >>_UPDATE_ROM\updater-script
@echo ifelse(is_mounted("/system"),package_extract_dir("system", "/system"),ui_print("/system NOT mounted!!")); >>_UPDATE_ROM\updater-script
@echo unmount("/system"); >>_UPDATE_ROM\updater-script
_TOOLS\Dos2Unix _UPDATE_ROM\updater-script

for /f %%r in ('dir /b _UPDATE_INPUT\*.zip') do (
 @echo Create "_UPDATE_ZIP\update_%%~nr-%version%_Signed.zip"
 if not '%BPU%'=='' (
  7za e _INPUT_ROM\miuinl_%%~nr-%version%.zip -o_UPDATE_INPUT\system build.prop -r -y
  _TOOLS\Unix2Dos _UPDATE_INPUT\system\build.prop 
  for /f %%l in (_UPDATE_INPUT\system\build.prop) do (
   @if '%%l'=='ro.build.version.incremental=%version%' (
    @echo ro.build.version.incremental=%forversion% >>"%~dp0_UPDATE_ROM\system\build.prop"
   ) else (
    @echo %%l >>"%~dp0_UPDATE_ROM\system\build.prop"
   )
  )
 )
 _TOOLS\Dos2Unix _UPDATE_ROM\system\build.prop 
 copy /y _UPDATE_INPUT\%%r _UPDATE_ROM\update_%%~nr-%version%.zip
 ::md _UPDATE_ROM\META-INF\com\google\android
 ::copy /y _UPDATE_ROM\updater-script _UPDATE_ROM\META-INF\com\google\android\updater-script
 7za a -tzip "%~dp0_UPDATE_ROM\update_%%~nr-%version%.zip" "%~dp0_UPDATE_ROM\system" -mx%complevel%
 ::7za a -tzip "%~dp0_UPDATE_ROM\update_%%~nr-%version%.zip" "%~dp0_UPDATE_ROM\META-INF" -mx%complevel%
 java -jar "_SIGN_ZIP\signapk.jar" "_SIGN_ZIP\testkey.x509.pem" "_SIGN_ZIP\testkey.pk8" "_UPDATE_ROM\update_%%~nr-%version%.zip" "_UPDATE_ZIP\update_%%~nr-%forversion%_Signed.zip"
 del /q "%~dp0_UPDATE_ROM\update_%%~nr-%version%.zip"
 del /q "%~dp0_UPDATE_ROM\system\build.prop"
 del /q "%~dp0_UPDATE_INPUT\system\build.prop"
)
for /f %%d in ('dir /b /a:d _UPDATE_ROM') do rd /s /q _UPDATE_ROM\%%d
for /f %%f in ('dir /b _UPDATE_ROM') do del /q _UPDATE_ROM\%%f

@echo update zip(s) created in _UPDATE_ZIP\*.*
pause
goto restart

:: ------------------------------
:: ------------------------------
:: ------------------------------
:: ------------------------------
:: ------------------------------
:: ------------------------------
:: ------------------------------
:automatic
if exist %logD% DEL %logD%
::
::
	set DoSpecial=MiuiCamera Torch FM Updater
	set _LANGUAGE=Dutch
::
::
::
:: rename _INPUT_ROM\*.zip to wanted names
for /f "tokens=1,2 delims=_" %%a in ('dir /b _INPUT_ROM\miuiandroid*.zip') do (
 @ren _INPUT_ROM\%%a_%%b miuinl_%%b
)

for /f "tokens=1-4,5* delims=_" %%a in ('dir /b _INPUT_ROM\miui_xj*.zip') do (
 @if '%%c'=='D2EXT' @ren _INPUT_ROM\%%a_%%b_%%c_%%d_%%e_%%f miuinl_Desire_%%c-%%e.zip
 @if '%%c'=='D2W'   @ren _INPUT_ROM\%%a_%%b_%%c_%%d_%%e_%%f miuinl_Desire_%%c-%%d.zip
 @if '%%c'=='A2SD'  @ren _INPUT_ROM\%%a_%%b_%%c_%%d_%%e_%%f miuinl_Desire_%%c-%%d.zip
)
for /f "tokens=1-4,5* delims=_" %%a in ('dir /b _INPUT_ROM\OTA*.zip') do (
 @if '%%a'=='OTA'   @ren _INPUT_ROM\%%a_%%b_%%c_%%d_%%e_%%f miuinl_Desire_OTA-%%d.zip
)
::
::ifframework
Set BeginTime=%time%
if exist _FLASHABLES\system\app\*.apk del /q _FLASHABLES\system\app\*.apk
if exist _FLASHABLES\system\framework\*.apk del /q _FLASHABLES\system\framework\*.apk

@echo.
@echo	Please DO NOT mix more MIUI version ROMs in _INPUT_ROM folder
@echo	YOU are responsible checking, this script can't possibly know of version conflicts
@echo	If different ROMs versions exist in _INPUT_ROM, wrong app's will be inserted in the ROMs
@echo	Move unwanted ROMs version out of _INPUT_ROM folder (or make them hidden)
@echo.
@echo.
@echo	rule-of-thumb: when translating new version of ROM: always drag new version ROM to next prompt,
@echo	 and all work-folders will be cleaned of all files
@echo.
@echo.
@echo	If YOU are NOTE SURE about content of _INPUT_ROM: check NOW
@echo.
@echo	You can exit by typing 'exit' to next prompt
@echo.
@echo	When you the press 'Enter'-key the current set of _INPUT_APK\*.apk files will be used
@echo	 to be inserted in the ROMs 
@echo.
set DoWork=
set /P DoWork=Drag ROM file from _INPUT_ROM here, type 'exit' or press [enter]-key : %=%
if '%DoWork%'=='exit' goto restart
if '%DoWork%'=='' goto nonewdecompilerecompile

if not exist %DoWork% (
@echo %DoWork% does NOT EXIST, please select ROM from _INPUT_ROM folder and drag in window
pause
goto automatic
)

if exist %DoWork% (
 @echo %DoWork%
 ::cleanup
 if exist _OUT_APK\*.apk del /q _OUT_APK\*.apk
 if exist _INPUT_APK\*.apk del /q _INPUT_APK\*.apk
 if exist _OUT_APK_ZIPALIGN\*.apk del /q _OUT_APK_ZIPALIGN\*.apk
 for /f %%D in ('dir /b /a:d _INPUT_APK') do rd /s /q _INPUT_APK\%%D
 for /f %%D in ('dir /b /a:d _TEMP') do rd /s /q _TEMP\%%D
 if exist _TEMP\*.* del /q _TEMP\*.*
 @echo cleaned all folders
 7za e %DoWork% -o_INPUT_APK *.apk -r -y
 @echo extracted *.apk from %DoWork% to _INPUT_APK\*.apk
 @echo Please add extra *.apk now in _INPUT_APK folder
 pause
 @echo %time%::ifframework >>%logD%
 if not exist "%~dp0_INPUT_APK\framework-res.apk" (
  @echo ERROR: framework-res.apk is not in "_INPUT_APK" folder
  pause
  goto restart
 )
 @echo installing framework file...
 set FRMWRK=
 set FRMWRK="_INPUT_APK\framework-res.apk"
 call 01framework-if.bat %INPUT%
 @echo DONE
 ::dont decompile-recompile if no translation file available
 for /f %%x in ('dir /b _INPUT_APK\*.apk') do (
  if not exist _TRANSLATE\%_LANGUAGE%\%%~nx del _INPUT_APK\%%x
 )
 for %%i IN (framework-res %DoSpecial%) DO if exist _INPUT_APK\%%i.apk del _INPUT_APK\%%i.apk

 ::decompile
 @echo %time%:decompile >>%logD%
 @echo decompiling apks...
 :: for loop call to 02decompileAPK
 FOR %%F IN (_INPUT_APK\*.apk) DO (
  @echo [*] %%F >>%logD%
  @echo decompiling %%F... >>%logD%
  call 02decompileAPK %%F 2>>%logD%
 )
 ::install translation files
 @echo %time%:install translation >>%logD%
 @echo additional translation(s)
 FOR /f %%F IN ('dir /b /a:d _TRANSLATE\%_LANGUAGE%') DO (
  @echo [*] %%F >>%logD%
  if exist _INPUT_APK\%%F xcopy /e /f /i /y _TRANSLATE\%_LANGUAGE%\%%F _INPUT_APK\%%F
 )
 ::recompile
 @echo %time%::recompile >>%logD%
 @echo recompiling apks...
 del /q %logR%
 :: for loop call to 03recompileAPK
 for /f "tokens=* delims= " %%a in ('dir _INPUT_APK /b /ad') do (
  if exist "_INPUT_APK\%%a\res\values-nl-rNL" (
   if exist "_TRANSLATE\%_LANGUAGE%\%%a\res\values-nl" (
    @echo Update "_INPUT_APK\%%a\res\values-nl-rNL" with "_TRANSLATE\%_LANGUAGE%\%%a\res\values-nl\*.*"
    copy /y  "_TRANSLATE\%_LANGUAGE%\%%a\res\values-nl\*.*" "_INPUT_APK\%%a\res\values-nl-rNL"
   )
  )
  @echo [*] %%a folder >>%logR%
  @echo recompiling %%a.apk... >>%logR%
  call 03recompileAPK %%a 2>>%logR%
 )
 @echo Please check if recompile is OK
 pause
 ::rebuildusable
 @echo %time%::rebuildusable >>%logD%
 @echo rebuilding apks...
 FOR %%F IN (_INPUT_APK\*.apk) DO call 04rebuildusableAPK %%F
)

:nonewdecompilerecompile
for %%i IN (framework-res %DoSpecial%) DO (
if exist _INPUT_APK\%%i.apk del /q _INPUT_APK\%%i.apk
if exist _OUT_APK\%%i.apk del /q _OUT_APK\%%i.apk
if exist _FLASHABLES\system\app\%%i.apk del /q _FLASHABLES\system\app\%%i.apk
if exist _FLASHABLES\system\framework\%%i.apk del /q _FLASHABLES\system\framework\%%i.apk
if exist _INPUT_APK\%%i rd /s /q _INPUT_APK\%%i
if exist _OUT_APK_ZIPALIGNED\%%i.apk del /q _OUT_APK_ZIPALIGNED\%%i.apk
)
::buildflashableall
@echo %time%::buildflashableall >>%logD%
:: do checks first
FOR %%F IN (_OUT_APK\*.apk) DO call 06buildflashable %%F
:: will use this for some other option
if exist "_FLASHABLES\system\app\framework-res.apk" (
COPY "_FLASHABLES\system\app\framework-res.apk" "_FLASHABLES\system\framework\framework-res.apk"
)
:: will use this for some other option
if exist "_FLASHABLES\system\app\framework-res.apk" (
DEL "_FLASHABLES\system\app\framework-res.apk"
)
@echo Building all ROMs in folder _INPUT_ROMS ...

:: ------------------------------
::automaticmiuinl
@echo %time%::automaticmiuinl >>%logD%
for /f %%r in ('dir /b _INPUT_ROM\*.zip') do (
  for %%i IN (framework-res %DoSpecial%) DO (
   if exist "_OUT_APK\%%i.apk" del "_OUT_APK\%%i.apk"
   if exist "_FLASHABLES\system\app\%%i.apk" DEL "_FLASHABLES\system\app\%%i.apk"
   if exist "_FLASHABLES\system\framework\%%i.apk" DEL "_FLASHABLES\system\framework\%%i.apk"
   if exist "_INPUT_APK\%%i.apk" del "_INPUT_APK\%%i.apk"
   if exist "_INPUT_APK\%%i" rd /s /q "_INPUT_APK\%%i"
   7za e _INPUT_ROM\%%r -o_INPUT_APK %%i.apk -r -y
   if not exist "_INPUT_APK\framework-res.apk" @echo check "_INPUT_APK\framework-res.apk" &pause
   if not exist "_INPUT_APK\%%i.apk" @echo Not in this ROM: "_INPUT_APK\%%i.apk"
   set FRMWRK=
   set FRMWRK="_INPUT_APK\framework-res.apk"
   call 01framework-if.bat %INPUT%
   if exist "_INPUT_APK\%%i.apk" (
    call 02decompileAPK "_INPUT_APK\%%i.apk"
    xcopy /e /f /i /y "_TRANSLATE\%_LANGUAGE%\%%i" "_INPUT_APK\%%i"
	
	for /f "tokens=1-3 delims=_-" %%a in ('dir /b _INPUT_ROM\%%r') do (
     @if '%%c'=='D2EXT' if exist "_TRANSLATE\%_LANGUAGE%\%%i.DIFF" @xcopy /e /f /i /y "_TRANSLATE\%_LANGUAGE%\%%i.DIFF" "_INPUT_APK\%%i"
     @if '%%c'=='D2W'   if exist "_TRANSLATE\%_LANGUAGE%\%%i.DIFF" @xcopy /e /f /i /y "_TRANSLATE\%_LANGUAGE%\%%i.DIFF" "_INPUT_APK\%%i"
     @if '%%c'=='A2SD'  if exist "_TRANSLATE\%_LANGUAGE%\%%i.DIFF" @xcopy /e /f /i /y "_TRANSLATE\%_LANGUAGE%\%%i.DIFF" "_INPUT_APK\%%i"
     @if '%%c'=='OTA'   if exist "_TRANSLATE\%_LANGUAGE%\%%i.DIFF" @xcopy /e /f /i /y "_TRANSLATE\%_LANGUAGE%\%%i.DIFF" "_INPUT_APK\%%i"
	)

    for /f %%d in ('dir /b /a:d _INPUT_APK\framework-res\res\*-zh*')  do rd /s /q _INPUT_APK\framework-res\res\%%d
    for /f %%d in ('dir /b /a:d _INPUT_APK\framework-res\res\*-mcc*') do rd /s /q _INPUT_APK\framework-res\res\%%d
    for /f %%d in ('dir /b /a:d _INPUT_APK\framework-res\res\*-rAU*') do rd /s /q _INPUT_APK\framework-res\res\%%d
    for /f %%d in ('dir /b /a:d _INPUT_APK\framework-res\res\*-rCA*') do rd /s /q _INPUT_APK\framework-res\res\%%d
    for /f %%d in ('dir /b /a:d _INPUT_APK\framework-res\res\*-rGB*') do rd /s /q _INPUT_APK\framework-res\res\%%d
    for /f %%d in ('dir /b /a:d _INPUT_APK\framework-res\res\*-rIE*') do rd /s /q _INPUT_APK\framework-res\res\%%d
    for /f %%d in ('dir /b /a:d _INPUT_APK\framework-res\res\*-rNZ*') do rd /s /q _INPUT_APK\framework-res\res\%%d
    if exist "_INPUT_APK\%%i\res\values-nl-rNL" (
     if exist "_TRANSLATE\%_LANGUAGE%\%%i\res\values-nl" (
      @echo Update "_INPUT_APK\%%i\res\values-nl-rNL" with "_TRANSLATE\%_LANGUAGE%\%%i\res\values-nl\*.*"
      copy /y  "_TRANSLATE\%_LANGUAGE%\%%i\res\values-nl\*.*" "_INPUT_APK\%%i\res\values-nl-rNL"
     )
    )
    ::if %%i==framework-res dir /b /a:d _INPUT_APK\framework-res\res
    call 03recompileAPK "_INPUT_APK\%%i"
    call 04rebuildusableAPK "_INPUT_APK\%%i.apk"
    call 06buildflashable "_OUT_APK\%%i.apk"
    if not exist "_FLASHABLES\system\app\%%i.apk" @echo check "_FLASHABLES\system\app\%%i.apk" &pause
   )
  )
 
 @echo %time%::buildflashableall >>%logD%
 :: do checks first
 :: will use this for some other option
 if exist "_FLASHABLES\system\app\framework-res.apk" (
  COPY "_FLASHABLES\system\app\framework-res.apk" "_FLASHABLES\system\framework\framework-res.apk"
 )
 :: will use this for some other option
 if exist "_FLASHABLES\system\app\framework-res.apk" (
  DEL "_FLASHABLES\system\app\framework-res.apk"
 )
 md "_TEMP\%%~nr"
 xcopy /e /f /i /y "_FLASHABLES\system" "_TEMP\%%~nr\system"
)
::-----------------------------------
:automaticnoprecompile
::
@echo ui_print(""); >_TEMP\updater-script.miuinl
@echo ui_print("************************************************"); >>_TEMP\updater-script.miuinl
@echo ui_print("*                                              *"); >>_TEMP\updater-script.miuinl
@echo ui_print("*   ##      ## ## ##  ## ##    ##   ## ##      *"); >>_TEMP\updater-script.miuinl
@echo ui_print("*   ###    ### ## ##  ## ##    ###  ## ##      *"); >>_TEMP\updater-script.miuinl
@echo ui_print("*   ## #### ## ## ##  ## ##    #### ## ##      *"); >>_TEMP\updater-script.miuinl
@echo ui_print("*   ##  ##  ## ## ##  ## ##    ## #### ##      *"); >>_TEMP\updater-script.miuinl
@echo ui_print("*   ##  ##  ## ## ##  ## ##    ##  ### ##      *"); >>_TEMP\updater-script.miuinl
@echo ui_print("*   ##      ## ##  ####  ##    ##   ## #####   *"); >>_TEMP\updater-script.miuinl
@echo ui_print("*                                              *"); >>_TEMP\updater-script.miuinl
@echo ui_print("************************************************"); >>_TEMP\updater-script.miuinl
@echo ui_print(""); >>_TEMP\updater-script.miuinl
::
for /f %%r in ('dir /b _INPUT_ROM\*.zip') do (
 if exist "%~dp0_TEMP\%%~nr\system\framework\framework-res.apk" (
  @echo [*] %%r >>%logD%
  @echo Building %%r ROM... >>%logD%
  COPY /y "_INPUT_ROM\%%r" "_TEMP\%%r"
  @echo Updating and Compressing "_TEMP\%%r"... >>%logD%
  @echo Updating and Compressing "_TEMP\%%r"...

  type _TEMP\updater-script.miuinl >_TEMP\updater-script
  7za e _TEMP\%%r -o_TEMP\%%~nr\META-INF\com\google\android updater-script -r -y
  _TOOLS\Unix2Dos _TEMP\%%~nr\META-INF\com\google\android\updater-script
  type _TEMP\%%~nr\META-INF\com\google\android\updater-script >>_TEMP\updater-script
  copy /y _TEMP\updater-script _TEMP\%%~nr\META-INF\com\google\android\updater-script
  _TOOLS\Dos2Unix _TEMP\%%~nr\META-INF\com\google\android\updater-script
  del /q _TEMP\updater-script
  7za a -tzip "%~dp0_TEMP\%%r" "%~dp0_TEMP\%%~nr\*" -mx%complevel%
 
  for %%d in (MIUIStats.apk) do 7za d _TEMP\%%r %%d -r
  for /f "tokens=1-3 delims=_-" %%a in ('dir /b _INPUT_ROM\%%r') do (
   @if '%%c'=='OTA' 7za d _TEMP\%%r media -r
  )

  @echo Signing "_TEMP\%%r" to _OUTPUT_ROM\%%~nr-signed.zip ...
  java -jar "_SIGN_ZIP\signapk.jar" "_SIGN_ZIP\testkey.x509.pem" "_SIGN_ZIP\testkey.pk8" "_TEMP\%%r" "_OUTPUT_ROM\%%~nr_Signed.zip"
 )
)
for %%i IN (framework-res %DoSpecial%) DO (
if exist _INPUT_APK\%%i.apk del /q _INPUT_APK\%%i.apk
if exist _OUT_APK\%%i.apk del /q _OUT_APK\%%i.apk
if exist _FLASHABLES\system\app\%%i.apk del /q _FLASHABLES\system\app\%%i.apk
if exist _FLASHABLES\system\framework\%%i.apk del /q _FLASHABLES\system\framework\%%i.apk
if exist _INPUT_APK\%%i rd /s /q _INPUT_APK\%%i
if exist _OUT_APK_ZIPALIGNED\%%i.apk del /q _OUT_APK_ZIPALIGNED\%%i.apk
)
@echo %time%::ready >>%logD%
set EndTime=%time%
echo Begin: %BeginTime%, End: %EndTime%
pause
goto :eof
:: ------------------------------
:: ------------------------------
:: ------------------------------
:: ------------------------------
:: ------------------------------
:: ------------------------------
:: ------------------------------
:ifframework
if not exist "%~dp0_INPUT_APK\framework-res.apk" (
@echo ERROR: framework-res.apk is not in "_INPUT_APK" folder
pause
goto restart
)
@echo installing framework file...
set FRMWRK=
set FRMWRK="_INPUT_APK\framework-res.apk"
call 01framework-if.bat %INPUT%
@echo DONE
pause
cls
goto restart
:installtranslation
set _LANGUAGE=Dutch
if exist %logD% DEL %logD%
@echo installing additional translation(s)...
:: for loop to copy additional translation folders
FOR /f %%F IN ('dir /b /a:d "_INPUT_APK"') DO (
@echo [*] %%F >>%logD%
@echo copying %%F... >>%logD%
@echo xcopy /e /f /i /y "_TRANSLATE\%_LANGUAGE%\%%F" "_INPUT_APK\%%F"
xcopy /e /f /i /y "_TRANSLATE\%_LANGUAGE%\%%F" "_INPUT_APK\%%F"
)
notepad %logD%
@echo DONE
goto restart
:removeunnes
if exist _INPUT_APK\gmail rd /s /q _INPUT_APK\gmail
if exist _INPUT_APK\miliao rd /s /q _INPUT_APK\miliao
if exist _INPUT_APK\vending rd /s /q _INPUT_APK\vending
for /f %%d in ('dir /b /a:d _INPUT_APK\framework-res\res\*-zh*') do rd /s /q _INPUT_APK\framework-res\res\%%d
for /f %%d in ('dir /b /a:d _INPUT_APK\framework-res\res\*-mcc*') do rd /s /q _INPUT_APK\framework-res\res\%%d
goto restart
:installaddapk
set FRMWRK=
set /P FRMWRK=Drag APK here to install as additional framework: %=%
if '%FRMWRK%'=='' goto :installaddapk
@echo installing additional framework file...
call 01framework-if.bat %FRMWRK%
@echo DONE
pause
cls
goto restart
:decompile
if exist %logD% DEL %logD%
@echo decompiling apks...
:: for loop call to 02decompileAPK
FOR %%F IN (_INPUT_APK\*.apk) DO (
@echo [*] %%F >>%logD%
@echo decompiling %%F... >>%logD%
call 02decompileAPK %%F 2>>%logD%
)
notepad %logD%
goto restart
:recompile
if exist %logR% DEL %logR%
@echo recompiling apks...
:: for loop call to 03recompileAPK
for /f "tokens=* delims= " %%a in ('dir _INPUT_APK /b/ad') do (
@echo [*] %%a folder >>%logR%
@echo recompiling %%a.apk... >>%logR%
call 03recompileAPK %%a 2>>%logR%
)
notepad %logR%
goto restart
:rebuildusable
@echo rebuilding apks...
FOR %%F IN (_INPUT_APK\*.apk) DO call 04rebuildusableAPK %%F
@echo DONE
pause
goto restart
:singledecompile
if exist %logD% DEL %logD%
set INPUT=
set /P INPUT=Drag APK here from _INPUT_APK folder: %=%
@echo [*] %INPUT% >>%logD%
@echo decompiling %INPUT%... >>%logD%
call 02decompileAPK %INPUT% 2>>%logD%
notepad %logD%
goto restart
:singlerecompile
if exist %logR% DEL %logR%
set INPUT=
set /P INPUT=Drag FOLDER here from _INPUT_APK folder: %=%
@echo [*] %INPUT% >>%logR%
@echo recompiling %INPUT%... >>%logR%
call 03recompileAPK %INPUT% 2>>%logR%
notepad %logR%
goto restart
::
:singlerecthenbuild
if exist %logR% DEL %logR%
set INPUT=
set /P INPUT=Drag FOLDER here from _INPUT_APK folder: %=%
@echo [*] %INPUT% >>%logR%
@echo recompiling %INPUT%... >>%logR%
call 03recompileAPK %INPUT% 2>>%logR%
set strfolder=%INPUT%
for /f "useback tokens=*" %%a in ('%strfolder%') do set strfolder=%%~a
set "strext=.apk"
set "strapkfile=%strfolder%%strext%"
@echo rebuilding %strapkfile%...
call 04rebuildusableAPK "%strapkfile%"
notepad %logR%
goto restart
::
:singlebuildusableapk
set INPUT=
set /P INPUT=Drag APK here from _INPUT_APK folder: %=%
@echo rebuilding %INPUT%...
call 04rebuildusableAPK %INPUT%
@echo DONE
pause
goto restart
:signzip
set INPUT=
set /P INPUT=Drag ZIP file to SIGN here: %=%
call 05signzip %INPUT%
echo DONE
pause
goto restart
:: --------------------------------------------
:copyresourcestxt
if exist "%~dp0_CUSTOM_RES\Copy_Resources.txt" (
for /f "tokens=1,2 delims=||" %%a in (%~dp0_CUSTOM_RES\Copy_Resources.txt) do (
if exist "%~dp0_INPUT_APK\%%a" Copy "%~dp0_INPUT_APK\%%a" "%~dp0_CUSTOM_RES\%%b"
echo Copied %~dp0_INPUT_APK\%%a %~dp0_CUSTOM_RES\%%b
)
echo DONE. Specified files in list copied from _INPUT_APK folders to _CUSTOM_RES!
)
pause
goto restart
:zipalignoutapks
FOR %%K IN (_OUT_APK\*.apk) DO (
zipalign -f -v 4 "%~dp0_OUT_APK\%%~nxK" "%~dp0_OUT_APK_ZIPALIGN\%%~nxK"
)
echo DONE
pause
goto restart
:movezipaligntoout
MOVE _OUT_APK_ZIPALIGN\*.apk _OUT_APK
echo DONE
pause
goto restart
:: --------------------------------------------
:adbdevices
adb devices
pause
goto restart
:adbinstall
set INPUT=
set /P INPUT=Drag APK to install here: %=%
if '%INPUT%'=='' goto :adbinstall
adb install %INPUT%
pause
goto restart
::
:adbpushsystemapp
set INPUT=
set /P INPUT=Drag APK to push to /system/app here: %=%
if '%INPUT%'=='' goto :adbpushsystemapp
adb push %INPUT% /system/app
echo DONE. "%INPUT%" pushed to /system/app
pause
goto restart
:adbpullsystemapp
adb pull /system/app "%~dp0_ADB/system/app"
pause
goto restart
:adbpulldataapp
adb pull /data/app "%~dp0_ADB/data/app"
pause
goto restart
:adbpullframework
adb pull /system/framework "%~dp0_ADB/system/framework"
pause
goto restart
:setadbsdfolder
set adbsdfolder=
set /P adbsdfolder=Type in the folder in sd card for adb interaction: %=%
if '%adbsdfolder%'=='' (
echo Folder name can not be empty, please type again...
goto :setadbsdfolder
)
goto restart
:pullfromadbsdfolder
echo Pulling files from "%adbsdfolderpath%"...
adb pull %adbsdfolderpath% "%~dp0_ADB%adbsdfolderpath%"
echo DONE, folder pulled to _ADB%adbsdfolderpath%
pause
goto restart
:pushfileadbsdfolder
set INPUT=
set xfile=
set /P INPUT=Drag file to push to "sd folder" here: %=%
if '%INPUT%'=='' goto :pushfileadbsdfolder
for /f "tokens=* delims= " %%f in ("!INPUT!") do (
set xfile=%%~NXf
::echo !xfile!
echo Pushing file to "%adbsdfolderpath%"...
adb push %INPUT% %adbsdfolderpath%/!xfile!
)
echo DONE, File path in sdcard is "%adbsdfolderpath%/!xfile!"...
pause
goto restart
::adb reboot
:adbreboot
echo  Reboot Phone?
echo    1 = Reboot to recovery
echo    2 = Reboot to bootloader
echo    3 = Reboot phone
echo    4 = Cancel
SET menureb=
SET /P menureb=Please make your decision: 
if '%menureb%'=='' goto :adbreboot
IF %menureb%==1 (goto rebootrecovery)
IF %menureb%==2 (goto rebootbootloader)
IF %menureb%==3 (goto rebootnormal)
IF %menureb%==4 (goto restart)
:: wrong choice made
@echo Wrong choice, please make a valid selection!
goto :adbreboot
:rebootrecovery
@echo Executing adb reboot recovery......
adb reboot recovery
@echo Done
goto restart
:rebootbootloader
@echo Executing adb reboot bootloader......
adb reboot bootloader
@echo Done
goto restart
:rebootnormal
@echo Executing adb reboot......
adb reboot
@echo Done
goto restart

:: --------------------------------------------
:buildflashable
:: do checks first
if not exist "%~dp0_FLASHABLES\Flashable_Update.zip" (
@echo ERROR: Flashable_Update.zip template file is not in "_FLASHABLES" folder
pause
goto restart
)
:: template present, proceed
FOR %%F IN (_OUT_APK\*.apk) DO call 06buildflashable %%F
:: will use this for some other option
if exist "%~dp0_FLASHABLES\system\app\framework-res.apk" (
COPY "%~dp0_FLASHABLES\system\app\framework-res.apk" "%~dp0_FLASHABLES\system\framework\framework-res.apk"
)
:: will use this for some other option
if exist "%~dp0_FLASHABLES\system\app\framework-res.apk" (
DEL "%~dp0_FLASHABLES\system\app\framework-res.apk"
)
COPY "%~dp0_FLASHABLES\Flashable_Update.zip" "%~dp0_FLASHABLES\TEMP_Flashable_Update.zip"
@echo Updating and Compressing "_FLASHABLES\Flashable_Update.zip"...
7za a -tzip "%~dp0_FLASHABLES\TEMP_Flashable_Update.zip" "%~dp0_FLASHABLES\system" -mx%complevel%
@echo Signing "_FLASHABLES\TEMP_Flashable_Update.zip"...
java -jar "%~dp0_SIGN_ZIP\signapk.jar" "%~dp0_SIGN_ZIP\testkey.x509.pem" "%~dp0_SIGN_ZIP\testkey.pk8" "%~dp0_FLASHABLES\TEMP_Flashable_Update.zip" "%~dp0_FLASHABLES\Flashable_Update_Signed.zip"
DEL "%~dp0_FLASHABLES\TEMP_Flashable_Update.zip"
@echo DONE
pause
cls
goto restart
:: -------------------------------
:setcomplevel
set /P tmpcomplevel=Enter Compression Level (0-9) : %=%
if '%tmpcomplevel%'=='' goto :setcomplevel
if %tmpcomplevel% GTR 9 (
echo Invalid compression level, choose between 0-9
goto :setcomplevel
)
if %tmpcomplevel% LSS 0 (
echo Invalid compression level, choose between 0-9
goto :setcomplevel
)
set complevel=%tmpcomplevel%
type "Settings/AutoAPKToolCompLevelSettings.ini" | find /v "complevelsettings=" > "Settings/AutoAPKToolCompLevelSettings.ini"
echo complevelsettings=[%tmpcomplevel%>> "Settings/AutoAPKToolCompLevelSettings.ini"
cls
goto restart
:: -------------------------------
:cleanflashable
echo  CLEAN FLASHABLE FILES?
echo    1 = YES
echo    2 = CANCEL
SET menunr=
SET /P menunr=Please make your decision: 
if '%menunr%'=='' goto :cleanflashable
IF %menunr%==1 (goto cfYES)
IF %menunr%==2 (goto cfNO)
:: wrong choice made
@echo Wrong choice, please make a valid selection!
goto cleanflashable
:cfYES
@echo Cleaning "_FLASHABLES\system\app" and "_FLASHABLES\system\framework"...
FOR %%A IN (_FLASHABLES\system\app\*.*) DO DEL %%A
FOR %%B IN (_FLASHABLES\system\framework\*.*) DO DEL %%B
@echo DONE
pause
:cfNO
goto restart
:cleaninput
echo  CLEAN INPUT FOLDER FILES?
echo    1 = YES
echo    2 = CANCEL
SET menuinput=
SET /P menuinput=Please make your decision: 
if '%menuinput%'=='' goto :cleaninput
IF %menuinput%==1 (goto cinputYES)
IF %menuinput%==2 (goto cinputNO)
:: wrong choice made
@echo Wrong choice, please make a valid selection!
goto cleaninput
:cinputYES
@echo Cleaning "_INPUT_APK"...
RMDIR /S /Q "_INPUT_APK"
mkdir "_INPUT_APK"
::FOR %%P IN (_INPUT_APK\*.*) DO DEL %%P
@echo DONE
pause
:cinputNO
goto restart
:cleanout
echo  CLEAN OUT FOLDER FILES?
echo    1 = YES
echo    2 = CANCEL
SET menunt=
SET /P menunt=Please make your decision: 
if '%menunt%'=='' goto :cleanout
IF %menunt%==1 (goto coYES)
IF %menunt%==2 (goto coNO)
:: wrong choice made
@echo Wrong choice, please make a valid selection!
goto cleanout
:coYES
@echo Cleaning all "_OUT.." folders ...
FOR %%C IN (_OUT_APK\*.*) DO DEL %%C
FOR %%D IN (_OUT_APKTOOL_DIST\*.*) DO DEL %%D
FOR %%E IN (_OUT_APKTOOL_DIST_ZIPALIGN\*.*) DO DEL %%E
FOR %%F IN (_OUT_APK_ZIPALIGN\*.*) DO DEL %%F
@echo DONE
pause
:coNO
goto restart
:clearlogs
echo  CLEAR ALL LOG FILES?
echo    1 = YES
echo    2 = CANCEL
SET menunC=
SET /P menunC=Please make your decision: 
if '%menunC%'=='' goto :clearlogs
IF %menunC%==1 (goto clearYES)
IF %menunC%==2 (goto clearNO)
:: wrong choice made
@echo Wrong choice, please make a valid selection!
goto clearlogs
:clearYES
if exist %logD% DEL %logD%
if exist %logR% DEL %logR%
:: if exist %logMain% DEL %logMain%
goto restart
:clearNO
goto restart
:viewlog
echo  SELECT LOG TO VIEW
echo    1 = Decompile Log
echo    2 = Recompile Log
echo    3 = Main Log
echo    4 = Back to Main Menu
SET menunl=
SET /P menunl=Please choose log to view: 
if '%menunl%'=='' goto :viewlog
IF %menunl%==1 (goto logDtxt)
IF %menunl%==2 (goto logRtxt)
IF %menunl%==3 (goto logMtxt)
IF %menunl%==4 (goto restart)
:: wrong choice made
@echo Wrong choice, please make a valid selection!
goto viewlog
:logDtxt
if exist %logD% (notepad %logD%)
else (
@echo Decompile log may have been cleared already.
)
goto viewlog
:logRtxt
if exist %logR% (notepad %logR%)
else (
@echo Recompile log may have been cleared already.
)
goto viewlog
:logMtxt
if exist %logMain% (notepad %logMain%)
else (
@echo Main log file may have been deleted.
goto viewlog
)
goto restart
:: ---------------------------------------------------
::DEV mods folders, detect what to apply
:devmods
set /a DirCnt=1
for /f "tokens=* delims= " %%b in ('dir _MODS /b/ad') do (
Call :getfolders "%%~nxb"
)
::For /F %%D in (%~dp0_MODS\*) Do Call :getfolders %%D
Goto :folder_menu
:getfolders 
Set Folder_%DirCnt%=%~n1
Set /a DirCnt += 1
Goto :quit
:folder_menu
Set /a DirCnt -= 1
If %DirCnt% GTR 1 (
Call :DoMenu
Goto :applythisfolder
) Else If %DirCnt% EQU 1 (	
Call :DoMenu
::set choice1=1
Goto :applythisfolder
) Else (
Echo No dev folders found in _MODS directory!
Pause
Goto :restart
)
:DoMenu
Echo.
Echo Select MODS Folder you want to apply:
for /L %%I in (1,1,%DirCnt%) Do Call :DoMenuItem %%I
Echo.
Call :AskDevChoice
Goto :quit
:DoMenuItem
for /f "delims== tokens=2" %%F in ('Set Folder_%1') Do Set This_Folder=%%F
Echo    %1) %This_Folder%
Goto :quit
:AskDevChoice
Echo Press Enter without entering choice to return to Main Menu
set choice1=
set /p choice1=Type the number of the Mod Folder you want to apply: 
if '%choice1%'=='' goto :restart
if not defined Folder_%choice1% goto :AskDevChoice
Goto :quit
:applythisfolder
for /f "delims== tokens=2" %%I in ('Set Folder_%choice1%') Do Set This_Folder=%%I
echo Applying "%This_Folder%" dev folder files...
xcopy /e /f /i /y "%~dp0_MODS\%This_Folder%" "%~dp0_INPUT_APK"
echo DONE. Mod folder "%This_Folder%" applied to _INPUT_APK files!
:: -----------------------------------------------------
if exist "%~dp0_MODS\Delete_%This_Folder%.txt" (
for /f "tokens=* delims= " %%a in (%~dp0_MODS\Delete_%This_Folder%.txt) do (
DEL %~dp0_INPUT_APK\%%a
echo Deleted %~dp0_INPUT_APK\%%a
)
echo DONE. Specified files in list deleted in _INPUT_APK folders!
)
pause
goto restart
:: -----------------------------------------------------
:errjava
cls
echo Java was not found, you will not be able to sign apks or use apktool
PAUSE
goto restart
:setapktoolversion
echo.
echo  SET APKTOOL VERSION: 
echo    1 = apktool 1.3.1
echo    2 = apktool 1.3.2
echo    3 = apktool 1.4.1
echo.
SET /P apkversion=Please choose apktool version:
if '%apkversion%'=='' goto :setapktoolversion
IF %apkversion%==1 (
set apktool=apktool_1.3.1.jar
goto setmyversion
)
IF %apkversion%==2 (
set apktool=apktool_1.3.2.jar
goto setmyversion
)
IF %apkversion%==3 (
set apktool=apktool_1.4.1.jar
goto setmyversion
)
:: wrong choice made
@echo Wrong choice, please make a valid selection!
goto :setapktoolversion
:setmyversion
type "Settings/AutoAPKToolMain.ini" | find /v "apktoolversion=" > "Settings/AutoAPKToolMain.ini"
echo apktoolversion=%apktool%>> "Settings/AutoAPKToolMain.ini"
goto restart
:: -----------------------------------------------------
:help
cls
echo.
echo FILES:
echo [*] _ADB, _INPUT_APK, _OUT_APK, _OUT_APKTOOL_DIST, _FLASHABLES (directories)
echo [*] _SIGN_ZIP, _CUSTOM_RES (directories)
echo [*] 7za.exe, roptipng.exe, aapt.exe, zipalign.exe (executables)
echo [*] apktool.bat
echo [*] AutoAPKToolMenu.bat
echo [*] commandprompt.bat
echo [*] 01framework-if.bat
echo [*] 02decompileAPK.bat
echo [*] 03recompileAPK.bat
echo [*] 04rebuildusableAPK.bat
echo [*] 05signzip.bat
echo [*] 06buildflashable.bat
echo [*] apktool_1.3.1.jar, apktool_1.3.2.jar, apktool_1.4.1.jar
echo [*] ReadMeHistory.txt
echo. 
echo HELP:
echo [*] Place APKs to decompile in _INPUT_APK folder
echo [*] Place framework-res.apk in _INPUT_APK folder as well
echo [*] Make your edits in the _INPUT_APK folder APKs
echo     - add language values-da, values-de, etc
echo [*] Auto Flashable is in _FLASHABLES folder
echo [*] Place the flashable zip to manually sign in _SIGN_ZIP folder
echo.
echo NOTES:
echo [*] _INPUT_APK folder will contain original APKs left untouched
echo [*] _OUT_APKTOOL_DIST will contain the raw output from apktool
echo [*] _OUT_APK will contain the original APK with new files:
echo     - classes.dex
echo     - resources.arsc
echo [*] _SIGN_ZIP contain the flashable zip output that will be signed
echo.
echo CREDITS:
echo [*] Brut.all for Apktool - a tool for reengineering apk files
echo [*] JesusFreke
echo [*] http://www.7-zip.org - 7za commandline version
echo [*] signapk.jar
echo.
pause
goto restart
:quit
