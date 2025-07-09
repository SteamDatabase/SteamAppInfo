This is a simple program that finds the Steam installation on disk, reads `appinfo.vdf` and `packageinfo.vdf` files and dumps appid/subid and their tokens.

This is mostly intended as an example on how to read these files.

## appinfo.vdf

### File Header

| Type   | Size | Version | Field Name          | Description |
|--------|------|---------|---------------------|-------------|
| uint32 | 4    |         | Magic               | Version identifier (see Version History) |
| uint32 | 4    |         | Universe            | Steam universe (always 1) |
| int64  | 8    | 41+     | String Table Offset | Offset to string table from file start |

### App Entry (Repeated)

| Type     | Size     | Version | Field Name    | Description |
|----------|----------|---------|---------------|-------------|
| uint32   | 4        |         | App ID        | Steam application ID |
| uint32   | 4        | 36+     | Size          | Size of data until end of binary VDF |
| uint32   | 4        |         | Info State    | State flags (2 = normal, 1 = prerelease/no info) |
| uint32   | 4        |         | Last Updated  | Unix timestamp of last update |
| uint64   | 8        | 38+     | PICS Token    | Product Info Change Set token |
| bytes    | 20       | 38+     | SHA-1 Hash    | Hash of text appinfo VDF |
| uint32   | 4        | 36+     | Change Number | Steam change number |
| ~~uint8~~| 1        | <38     | Section Type  | Section identifier (repeated until 0x08) |
| bytes    | 20       | 40+     | Binary SHA-1  | Hash of binary VDF data |
| bytes    | variable |         | Binary VDF    | VDF data in binary format |

Repeated read until the first uint32 (App ID) is zero (which is the file footer).

### String Table (Version 41+)

Located at the offset specified in the file header:

| Type             | Size     | Version | Field Name   | Description |
|------------------|----------|---------|--------------|-------------|
| uint32           | 4        | 41+     | String Count | Number of strings in table |
| null-terminated  | variable | 41+     | Strings      | Array of null-terminated strings |

This string table is used to deduplicate strings in all the binary vdf blobs, so it is required to parse this string table first,
and then parse the binary keyvalues with this string table.

### Version History

| Version | Magic Number | Start Date      | Description |
|---------|--------------|-----------------|-------------|
| 36      | `0x24445606` | circa 2011      | Added size field |
| 37      | `0x25445607` | circa 2012      | Same structure as v36 |
| 38      | `0x26445607` | circa 2013      | Added PICS token and SHA hash |
| 39      | `0x27445607` | circa 2017      | Same as v38 but unified VDF structure |
| 40      | `0x28445607` | Dec 2022        | Added binary VDF SHA1 hash validation |
| 41      | `0x29445607` | June 2024       | Added string table with offset pointer |

*For parsing older formats not implemented here, [see this repo](https://github.com/eepycats/appinfo-parser/).*

### SHA-1 Hash Fields

The appinfo format includes two different hash fields that serve different purposes:

#### SHA-1 Hash
This field contains the SHA-1 hash of the raw `CMsgClientPICSProductInfoResponse.AppInfo.buffer` from Steam's internal protobuf protocol, excluding the last byte (which is always `\x00`). This hash is used by Steam for downloading appinfo from CDN via HTTP and corresponds to the hash provided when the Steam client requests appinfo from the server.

This hash cannot be easily recalculated from the VDF data alone, as it represents the hash of the original protobuf buffer that Steam received from the server, not a hash of the local VDF representation.

#### Binary SHA-1 Hash (20 bytes, version 40+)
This field contains the SHA-1 hash of the binary VDF data that follows immediately after this field. This hash can be calculated directly from the VDF data and is used to verify the integrity of the local binary VDF content.

## packageinfo.vdf

### File Header

| Type   | Size | Version | Field Name | Description |
|--------|------|---------|------------|-------------|
| uint4  | 4    |         | Magic      | Version identifier (see Version History) |
| uint32 | 4    |         | Universe   | Steam universe (always 1) |

### Package Entry (Repeated)

| Type     | Size     | Version | Field Name    | Description |
|----------|----------|---------|---------------|-------------|
| uint32   | 4        |         | Package ID    | Steam package/subscription ID |
| bytes    | 20       |         | SHA-1 Hash    | Hash of package data |
| uint32   | 4        |         | Change Number | Steam change number |
| uint64   | 8        | 40+     | PICS Token    | Product Info Change Set token |
| bytes    | variable |         | Binary VDF    | VDF data in binary format |

Repeated read until the first uint32 (Package ID) is `0xFFFFFFFF` (which is the file footer).

### Version History

| Version | Magic Number | Start Date      | Description |
|---------|--------------|-----------------|-------------|
| 39      | `0x27555606` | Before Apr 2020 | Basic format |
| 40      | `0x28555606` | Apr 2020        | Added PICS Token field |
