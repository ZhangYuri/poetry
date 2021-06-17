# Poetry References Library

This library includes the .NET implementation to read data from [chinese-poetry project](https://github.com/chinese-poetry/chinese-poetry).

[![MIT licensed](./docs/assets/badge_lisence_MIT.svg)](https://github.com/nuscien/trivial/blob/master/LICENSE)

![.NET 6](./docs/assets/badge_NET_6.svg)
![.NET 5](./docs/assets/badge_NET_5.svg)
![.NET Core 3.1](./docs/assets/badge_NET_Core_3_1.svg)
![.NET Standard 2.0](./docs/assets/badge_NET_Standard_2_0.svg)
![.NET Framework 4.8](./docs/assets/badge_NET_Fx_4_8.svg)
![.NET Framework 4.6.1](./docs/assets/badge_NET_Fx_4_6_1.svg)

## Setup

You need make sure the original data folder `chinese-poetry` is in the directory that your app is in.

## SDK

```csharp
using Poetry.Chinese;
```

You can load following poetry collections.

- `await LoadPrimateSchoolAsync()` Get the Primate School 蒙学.
- `await LoadTangPoetryAsync()` Get the Tang Poetry 唐诗.
- `await LoadSongPoetryAsync()` Get the Song Poetry 宋诗.
- `await LoadSongLyricsAsync()` Get the Song Lyrics 宋词.
- `await LoadYuanVersesAsync()` Get the Yuan Verses 元曲.
- `await LoadBookOfSongsAsync()` Get the Book of Songs 诗经.
- `await LoadFourBooksFiveClassicsAsync()` Get the Four Books and the Five Classics 四书五经.
- `await LoadConfucianAnalectsAsync()` Get the Confucian Analects 论语.
- `await LoadChuSongsAsync()` Get the Chu Songs 楚辞.
- `await LoadHuajianAnthologyAsync()` Get the Huajian Anthology 花间集.
- `await LoadSouthernTangPoetryAsync()` Get the Southern Tang Poetry 南唐词.
- `await LoadCaocaoAnthologyAsync()` Get the Caocao Anthology 曹操诗集.

## CLI

You can open the `Poetry.Chinese.exe` application file directly or run following on Windows.

```sh
.\Poetry.Chinese
```

And for macOS and Linux, you can execute following command.

```sh
dotnet Poetry.Chinese.dll
```

## Build

Open `Poetry.sln` solution file and build by Visual Studio. The output libraries and files will be in `bin\Debug` or `bin\Release` directory.
