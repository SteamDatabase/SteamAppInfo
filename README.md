This is a simple program that finds the Steam installation on disk, reads `appinfo.vdf` and `packageinfo.vdf` files and dumps appid/subid and their tokens.

This is mostly intended as an example on how to read these files.

## appinfo.vdf

### File Header

| Type   | Size | Version | Field Name          | Description |
|--------|------|---------|---------------------|-------------|
| uint8  | 1    |         | Version             | File format version |
| bytes  | 3    |         | Magic               | Always `0x44 0x56 0x07` |
| uint32 | 4    |         | Universe            | Steam universe (always 1) |
| int64  | 8    | 41+     | String Table Offset | Offset to string table from file start |

### App Entry (Repeated)

| Type     | Size     | Version | Field Name    | Description |
|----------|----------|---------|---------------|-------------|
| uint32   | 4        |         | App ID        | Steam application ID |
| uint32   | 4        |         | Size          | Size of data until end of binary VDF |
| uint32   | 4        |         | Info State    | State flags (2 = normal, 1 = prerelease/no info) |
| uint32   | 4        |         | Last Updated  | Unix timestamp of last update |
| uint64   | 8        |         | PICS Token    | Product Info Change Set token |
| bytes    | 20       |         | SHA-1 Hash    | Hash of text appinfo VDF |
| uint32   | 4        |         | Change Number | Steam change number |
| bytes    | 20       | 40+     | Binary SHA-1  | Hash of binary VDF data |
| bytes    | variable |         | Binary VDF    | VDF data in binary format |

Repeated read until the first uint32 (App ID) is zero (which is the file footer).

### File Footer

| Type   | Size | Version | Field Name | Description |
|--------|------|---------|------------|-------------|
| uint32 | 4    |         | EOF Marker | Always `0x00000000` |

### String Table (Version 41+)

Located at the offset specified in the file header:

| Type             | Size     | Version | Field Name   | Description |
|------------------|----------|---------|--------------|-------------|
| uint32           | 4        | 41+     | String Count | Number of strings in table |
| null-terminated  | variable | 41+     | Strings      | Array of null-terminated strings |

### Version History

- **Version 39** (0x27): Before December 2022
- **Version 40** (0x28): Added binary VDF SHA1 hash field (December 2022)
- **Version 41** (0x29): Added string table with offset pointer in header (June 2024)

## packageinfo.vdf

### File Header

| Type   | Size | Version | Field Name | Description |
|--------|------|---------|------------|-------------|
| uint8  | 1    |         | Version    | File format version |
| bytes  | 3    |         | Magic      | Always `0x55 0x56 0x06` |
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

### File Footer

| Type   | Size | Version | Field Name | Description |
|--------|------|---------|------------|-------------|
| uint32 | 4    |         | EOF Marker | Always `0xFFFFFFFF` |

### Version History

- **Version 39** (0x27): Before April 2020
- **Version 40** (0x28): Added PICS Token field (April 2020)
