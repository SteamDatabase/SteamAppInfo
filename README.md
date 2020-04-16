## appinfo.vdf
```
uint32   - MAGIC: "'DV\x07"
uint32   - UNIVERSE: 1
---- repeated app sections ----
uint32   - AppID
uint32   - size // appears to be a legacy offset
uint32   - infoState // mostly 2, sometimes 1 (may indicate prerelease or no info)
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
uint32   - MAGIC: "'UV\x06"
uint32   - UNIVERSE: 1
---- repeated package sections ----
uint32   - PackageID
20bytes  - SHA1
uint32   - changeNumber
uint64   - picsToken // added in April 2020, magic not changed
variable - binary_vdf
---- end of section ---------
uint32   - EOF: 0xFFFFFFFF
```
