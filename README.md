This is a simple program that finds the Steam installation on disk, reads `appinfo.vdf` and `packageinfo.vdf` files and dumps appid/subid and their tokens.

This is mostly intended as an example on how to read these files.

## appinfo.vdf
```
uint32   - MAGIC: 28 44 56 07
uint32   - UNIVERSE: 1
---- repeated app sections ----
uint32   - AppID
uint32   - size // until end of binary_vdf
uint32   - infoState // mostly 2, sometimes 1 (may indicate prerelease or no info)
uint32   - lastUpdated
uint64   - picsToken
20bytes  - SHA1 // of text appinfo vdf, as seen in CMsgClientPICSProductInfoResponse.AppInfo.sha
uint32   - changeNumber
20bytes  - SHA1 // of binary_vdf, added in December 2022
variable - binary_vdf
---- end of section ---------
uint32   - EOF: 0
```

## appinfo.vdf (before december 2022)
```
uint32   - MAGIC: 27 44 56 07
uint32   - UNIVERSE: 1
---- repeated app sections ----
uint32   - AppID
uint32   - size
uint32   - infoState
uint32   - lastUpdated
uint64   - picsToken
20bytes  - SHA1
uint32   - changeNumber
variable - binary_vdf
---- end of section ---------
uint32   - EOF: 0
```

## packageinfo.vdf
```
uint32   - MAGIC: 28 55 56 06
uint32   - UNIVERSE: 1
---- repeated package sections ----
uint32   - PackageID
20bytes  - SHA1
uint32   - changeNumber
uint64   - picsToken // added in April 2020
variable - binary_vdf
---- end of section ---------
uint32   - EOF: 0xFFFFFFFF
```

## packageinfo.vdf (before april 2020)
```
uint32   - MAGIC: 27 55 56 06
uint32   - UNIVERSE: 1
---- repeated package sections ----
uint32   - PackageID
20bytes  - SHA1
uint32   - changeNumber
variable - binary_vdf
---- end of section ---------
uint32   - EOF: 0xFFFFFFFF
```
