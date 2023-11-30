# sbox-manipulator

This was a third-party scene view tool for [s&box](https://sbox.facepunch.com/), made before the first-party offering. Like Unity Engine's scene view, you can edit objects while the game is running.

Features:
- Gizmos for translation, rotation and scale (uniform scaling only, due to a s&box limitation)
- Global and local manipulation
- Movable pivot for extra freedom in moving objects
- Configurable key bindings
- Selecting an object opens it in the s&box editor's built-in inspector window

Secret sauce:
- Uses some reflection hacks to get access to engine functions s&box did not expose
- Uses simple custom shaders for gizmos

It was made for an older version of s&box and no longer works with the latest engine APIs, so here's a few videos:

[![Video playlist](https://i.ytimg.com/vi/xnTNiqt4_CA/maxresdefault.jpg)](https://www.youtube.com/playlist?list=PLexWV3KyRjzHtXmqAkdQ-mhwzc3-VT46x)