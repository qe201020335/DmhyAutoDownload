# DmhyAutoDownload
An anime content subcriptor and automatic downloader for [dmhy](https://www.dmhy.org/). Supports sending downloads via AriaRPC.

## Documentation

### Deloy
- Clone this repo
- Copy `docker-compose.template.yml` to `docker-compose.yml`
- Edit `appsettings.override.json` and `docker-compose.yml` to your specific configuration
- `docker compose up -d`

### Api
Currently, a front-end is not available but a swagger ui can be found at `/index.html` in the development environment.

### Packages
| Name | Description |
|---|---|
| `DmhyAutoDownload.AriaRPC` |  A custom AriaRPC client implemented using [.NET StreamJsonRpc](https://github.com/microsoft/vs-streamjsonrpc) |
| `DmhyAutoDownload.Core` | The core logic of the project |
| `DmhyAutoDownload.Extensions` | Shared extension methods |
| `DmhyAutoDownload` | Program entry point |

