# Marching-Cubes-VR

Created application is used to create scenes 3D. Scenes consist of the terrain and objects placed on it. The application is dedicated to the Oculus Rift platform.

| ![example_scene_0](https://user-images.githubusercontent.com/29755810/52974013-5ee03480-33c0-11e9-96a7-b833c76a23cf.png) |
|:--:| 
| *An example of scene created using the application* |

Marching cubes algorithm allows to create triangle grid using the data from uniformly distributed in space samples. Each of the samples contains information whether it is inside or outside the model. 

The application has four modes:
 - Main mode
 - Scene mode
 - Terrain mode
 - Model mode
 
In terrain and model modes user gives shape to the selected object. The working tool is a brush with variable color, size, shape and type of applied changes. The scene mode allows you to arrange created models on terrain; manipulation and edition already set objects.

The application was written within engineering thesis at the MiNI Faculty (WUT) in collaboration with [McThrok](https://github.com/McThrok).

## Requirements

- Unity 2017 (or newer)
- Oculus Rift + Oculus Touch Controller

## User's manual

The buttons names used hereinafter are presented in the picture below

![oculus_touch-controllers](https://user-images.githubusercontent.com/29755810/52973238-a9ac7d00-33bd-11e9-9ff9-b415cb6a2b71.png)

### General menu rules

Detailed desriptions of the menus can be found later in this section. All menus have been created in accordance to one convention. The common rules for all menus are describet below.

Menus are objects ''attached'' to controllers. Thumbsticks are used to navidate through menus. **LThumbstick** is ised to navigate throught the left menus:
- right - open menu
- left - close menu

**RThumbstick** is ised to navigate throught the right menus:
- left - open menu
- right - close menu

Navigation when menu open:
- up, down - change active menu position
- click - select an active position

| ![menu_crop](https://user-images.githubusercontent.com/29755810/52974222-170ddd00-33c1-11e9-9e21-fafd2ff4fa80.png) |
|:--:| 
| *Sample menu view* |

#### Example

// TODO exapmple explanation

| ![menu_right_scene](https://user-images.githubusercontent.com/29755810/52978972-dff4f700-33d3-11e9-8ec2-f18906ce1dac.png) | ![menu_right_scene_0](https://user-images.githubusercontent.com/29755810/52978973-dff4f700-33d3-11e9-8a3a-7e1d1c092392.png) |
![menu_right_scene_1](https://user-images.githubusercontent.com/29755810/52978974-dff4f700-33d3-11e9-8b08-ce6e9e4cd4d3.png) | 
|:-------------:|:-------------:|:-----:|
|(a)|(b)|(c)|

| ![menu_right_scene_2](https://user-images.githubusercontent.com/29755810/52978975-dff4f700-33d3-11e9-8971-83e66fbd8040.png) |
![menu_right_scene_3](https://user-images.githubusercontent.com/29755810/52978976-e08d8d80-33d3-11e9-8324-5a67dcd709f8.png) |
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;|
|:-------------:|:-------------:|:-----:|
|(d)|(e)||

#### Menu positions types

// TODO

### Main mode

// TODO

### Scene mode

// TODO

### Terrain mode

// TODO

### Model mode
