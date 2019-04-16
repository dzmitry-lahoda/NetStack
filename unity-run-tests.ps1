$path = "C:\Program Files\Unity\Hub\Editor\2018.3.5f1\Editor\Unity.exe"
Start-Process -FilePath $path -ArgumentList "-runTests -projectPath . -testResults ./results.xml -testPlatform Android"