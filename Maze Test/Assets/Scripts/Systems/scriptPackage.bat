@echo off
rmdir /s /q "Build.GameClient\Profane.GameClient_BackUpThisFolder_ButDontShipItWithYourGame"
rmdir /s /q "Build.Hub\Profane.Hub_BackUpThisFolder_ButDontShipItWithYourGame"
rmdir /s /q "Build.Simulation\Profane.Simulation_BackUpThisFolder_ButDontShipItWithYourGame"

7z a -tzip builds-STORY-NUMBER.zip Build.GameClient\ Build.Hub\ Build.Simulation\ run.bat