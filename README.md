The current format is as follows:

Credits to https://github.com/ValvePython/vdf/issues/13#issuecomment-321700244

### appinfo.vdf
```
uint32   - MAGIC: "'DV\x07"
uint32   - UNIVERSE: 1
---- repeated app sections ----
uint32   - AppID
uint32   - size
uint32   - infoState 
uint32   - lastUpdated
uint64   - accessToken
20bytes  - SHA1
uint32   - changeNumber
variable - binary_vdf
---- end of section ---------
uint32   - EOF: 0
```

### packageinfo.vdf
```
uint32   - MAGIC: "'UV\x06"
uint32   - UNIVERSE: 1
---- repeated package sections ----
uint32   - PackageID
20bytes - SHA1
uint32   - changeNumber
uint64   - accessToken
variable - binary_vdf
---- end of section ---------
uint32   - EOF: 0xFFFFFFFF
```
