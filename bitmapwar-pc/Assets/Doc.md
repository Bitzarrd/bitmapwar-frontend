第一步，需要使用websocket.connection链接到服务器
测试服链接地址：ws://34.225.3.60:3000/
```javascript
websocket.connect("ws://34.225.3.60:3000/")
```

第二步，接受服务器发来的初始化消息，使用初始化消息的内容将地图创建
并且监听士兵移动消息，奖励结算消息，大奖爆灯消息

第三步，当用户登录钱包的时候向服务器发送登录消息，并监听登录成功消息

第四步，当用户点击分享是发送分享领奖消息，并且监听分享领奖成功消息

第五步，当用户投放士兵，发送投放士兵消息，并且监听投放士兵成功消息

##Reload初始化接口
方向：Server-->Client

字段：

| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'Reload'  |
| grid  | string  | 地图格子数据，需要解压缩，详见数据解压.md  |
| gridWidth  | int  | 格子宽度  |
| gridHeight  | int  | 格子高度  |
| players  | Player[]  | 当前玩家列表  |
| next_round  | int  | 下一回合开始时间  |
| statistics  | Statistics  | 统计信息  |
| stop_time  | int  | 本回合结束时间  |
| last_rank  | LastRank  | 上一回合比赛结果  |
| jackpot  | string  | 奖池  |

##登录
# 方向：Client-->Server
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'Login'  |
| address  | string  | 钱包地址  |

# 方向：Server-->Client
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'LoginSuccess'  |
| user  | User  | 用户信息  |
| has_login_gift  | bool  | 是否有今日登录奖励  |

##分享领取奖励
# 方向：Client-->Server
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'Share'  |
| owner  | string  | 钱包地址  |

# 方向：Server-->Client
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'ShareSuccess'  |
| user  | User  | 分享后的用户数据  |


# 方向：Server-->Client
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'GameStarted'  |
| gridWidth  | int  | 格子宽度  |
| gridHeight  | int  | 格子高度  |
| turn  | int  | 当前走到第几步  |
| start_time  | int  | 本回合开始时间  |
| stop_time  | int  | 本回合结束时间  |
| players  | Player[]  | 当前玩家列表  |

# 方向：Client-->Server
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'JoinGame2'  |
| map_id  | int  | 地图ID  |
| virus  | int  | 投入的士兵数量  |

# 方向：Server-->Client
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'JoinedGameSuccess'  |
| player  | Player  | 投放玩家数据  |
| user  | User  | 用户数据  |

# 方向：Server-->Client
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'Update'  |
| payload  | Cell[]  | 变更的地块信息  |
| turn  | int  | 当前走到第几步  |
| statistics  | Statistics  | 统计信息  |

# 方向：Server-->Client
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'Settlement'  |
| statistics  | Statistics  | 统计信息  |
| next_round  | int  | 下一回合开始时间  |
| user  | User  | 用户信息  |
| earning  | string  | 收益  |
| rank  | User[]  | 用户排名  |

# 方向：Server-->Client
## 字段
| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| method  | string  | 固定为'JackpotLightUp'  |
| land  | int  | 地块总数  |
| jackpot  | string  | 奖池  |
| team  | string  | 获胜队伍  |
| user  | User  | 爆灯用户的用户数据  |

# 以下是数据结构

Cell定义了某一个格子的变更信息

| 表头1 | 表头2 | 表头3 |
|-------|-------|-------|
| x  | int  | 格子所在的行数  |
| y  | int  | 格子所在的列数  |
| color  | string  | 格子的颜色  |

| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| owner  | string  | 钱包地址  |
| profit  | string  | 收益  |
| statistics.land  | int  | 地块总数  |

Player表示单个在场玩家的数据，每次投入士兵就会创建一个

| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| bitmap  | string  | 地块ID  |
| color  | string  | 颜色  |
| init_virus  | int  | 投入的士兵数量  |
| virus  | int  | 当前的士兵数量  |
| loss  | int  | 损失的士兵数量  |
| land  | int  | 地块总数  |
| x  | int  | 格子所在的行数  |
| y  | int  | 格子所在的列数  |
| owner  | string  | 钱包地址  |

4个阵营的统计信息

| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| team  | string  | 颜色  |
| virus  | int  | 当前士兵数量  |
| lose  | int  | 损失的士兵数量  |
| land  | int  | 当前地块数量  |

User定义了一个用户的数据，每个钱包有且仅有一个

| 字段名 | 字段类型 | 字段说明 |
|-------|-------|-------|
| address  | string  | 钱包地址  |
| profit  | string  | 可领取的收益总数  |
| virus  | int  | 当前拥有，但是未投入地图的士兵总数  |