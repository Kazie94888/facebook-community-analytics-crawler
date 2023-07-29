@echo off

set projectPath=D:\Workspace\lookon.vn.bo\lookon.vn.bo\src
set publishScriptPath=D:\Workspace\lookon.vn.bo\lookon.vn.bo\publish-scripts

set env=live
set password=)2Ytj#%%-bZZ(bC,+
rem timeout /t 3

echo ________________________________
echo *                              *
echo *      -Start to publish-      *
echo *      -ENV: %env%             *
echo *______________________________*  
echo -PASSWORD: %password% 

cd %projectPath%\LookOn.HttpApi.Host\
msbuild /p:Configuration=Release /p:DeployOnBuild=True /p:AllowUntrustedCertificate=True /p:PublishProfile=VPS248_Deploy.pubxml /p:EnvironmentName=Production
 
cd %projectPath%\LookOn.IdentityServer\  
msbuild /p:Configuration=Release /p:DeployOnBuild=True /p:AllowUntrustedCertificate=True /p:PublishProfile=VPS248_Deploy.pubxml /p:EnvironmentName=Production 
 
cd %projectPath%\LookOn.Web\ 
msbuild /p:Configuration=Release /p:DeployOnBuild=true /p:AllowUntrustedCertificate=True /p:PublishProfile=VPS248_Deploy.pubxml /p:EnvironmentName=Production 
 

echo ________________________________
echo *                              *
echo *      -Done publishing-       *
echo *______________________________*
Pause