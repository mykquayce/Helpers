on the Elgato devices, the endpoint `/elgato/lights` returns different JSON for the different makes/models of light

Keylight returns:
```json
{"numberOfLights":1,"lights":[{"on":0,"brightness":50,"temperature":143}]}
```
|key|type|value|
|-|:-:|:-:|
|on|int|0..1|
|brightness|int|0-100|
|temperature|int|143-344|

Lightstrip returns:
```json
{"numberOfLights":1,"lights":[{"on":0,"hue":82.0,"saturation":57.0,"brightness":30}]}
```
|key|type|value|
|-|:-:|:-:|
|on|int|0..1|
|hue|int|0-360|
|saturation|double|0-100|
|brightness|double|0-100|
