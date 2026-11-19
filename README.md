# 弹幕游戏原型（Unity 2D · 像素风）

一个可以直接跑起来的弹幕射击原型：2D、像素风、自己发射子弹、躲避敌方弹幕。
**所有视觉都是运行时程序化生成的像素 Sprite，不需要任何美术资源。**

## 操作

| 按键 | 功能 |
| --- | --- |
| WASD / 方向键 | 移动 |
| Shift | 慢速精确移动（判定点变亮） |
| 空格 / Z / J | 射击 |
| X / K | 清屏炸弹（2 发） |
| R | GameOver 后重开 |

## 已实现的系统

- `BulletPool`：子弹对象池，统一驱动移动/回收/圆形碰撞，不走物理引擎
- `BulletPatterns`：环形、扇形、自机狙、自机狙扇形等弹幕模板
- `Enemy` + `EnemyFactory`：5 种攻击模式（环形爆发 / 自机狙扇形 / 螺旋 / 狙击 / 双环），入场-停留-退场协程
- `PlayerController`：移动、慢速、射击、判定点、无敌帧、受击清弹
- `LevelDirector`：波次刷怪，难度随波数递增
- `GameManager` + `HUD`：分数、生命、炸弹、GameOver、震屏
- `ExplosionFx`：一次性像素爆散特效

## 后续可以加的东西

- Boss 战（多阶段弹幕、血条）
- 玩家子弹强化 / 道具掉落
- 音效与 BGM
- 竖版 9:16 布局（把相机 aspect 改成窄屏即可，边界是自适应的）

## 如果报 Input 相关错误

项目如果用了新版输入系统并禁用了旧输入：
Project Settings → Player → **Active Input Handling** 改成 **Both**（或 Input Manager (Old)），重启编辑器。
