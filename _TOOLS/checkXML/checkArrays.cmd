@echo off


set EnglishLocation=D:\AutoAPKTool2.0.3\_INPUT_APK
set DutchLocation=D:\GitRepository\MIUI-Dutch\Translations-Dutch\Dutch


for /f %%f in ('dir /b /a:d %EnglishLocation%') do (
 :: for *.apk
 if exist %DutchLocation%\%%f\res\values-nl\arrays.xml (
  echo Check: %DutchLocation%\%%f\res\values-nl\arrays.xml
  if exist %EnglishLocation%\%%f\res\values-en\arrays.xml (
   checkArrays.exe %EnglishLocation%\%%f\res\values-en\arrays.xml %DutchLocation%\%%f\res\values-nl\arrays.xml
  ) else (
   checkArrays.exe %EnglishLocation%\%%f\res\values\arrays.xml %DutchLocation%\%%f\res\values-nl\arrays.xml
  )
 )
 :: for framework-res.apk
 if exist %DutchLocation%\%%f\res\values-nl-rNL\arrays.xml (
  echo Check: %DutchLocation%\%%f\res\values-nl-rNL\arrays.xml
  checkArrays.exe %EnglishLocation%\%%f\res\values\arrays.xml %DutchLocation%\%%f\res\values-nl-rNL\arrays.xml
 )
)

echo Check XML is READY!
pause