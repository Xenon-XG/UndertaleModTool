using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.Windows
{
    public record GameVersion(uint Major, uint Minor, uint Release) : IComparable<GameVersion>
    {
        public static implicit operator GameVersion((uint, uint, uint) verTuple)
        {
            return new(verTuple.Item1, verTuple.Item2, verTuple.Item3);
        }

        public int CompareTo(GameVersion other)
		{
			int cmp = Major.CompareTo(other.Major);
			if (cmp != 0)
                return cmp;

			cmp = Minor.CompareTo(other.Minor);
			if (cmp != 0)
                return cmp;

			return Release.CompareTo(other.Release);
		}
    }

    public class TypesForVersion
    {
        public GameVersion Version { get; set; }
        public GameVersion BeforeVersion { get; set; } = new(uint.MaxValue, uint.MaxValue, uint.MaxValue);
        public (Type, string)[] Types { get; set; }
    }

    public static class UndertaleResourceReferenceMap
    {
        // Don't forget to update `referenceableTypesOrig` as well
        private static readonly Dictionary<Type, TypesForVersion[]> typeMap = new()
        {
            {
                typeof(UndertaleSprite),
                new[]
                {
                    new TypesForVersion
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleGameObject), "游戏对象")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleRoom.Tile), "房间图块"),
                            (typeof(UndertaleRoom.SpriteInstance), "房间精灵实例"),
                            (typeof(UndertaleRoom.Layer.LayerBackgroundData), "房间背景图层")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 2, 1),
                        Types = new[]
                        {
                            (typeof(UndertaleTextureGroupInfo), "纹理组")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2023, 2, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleParticleSystemEmitter), "粒子系统发射器")
                        }
                    }
                }
            },
            {
                typeof(UndertaleBackground),
                new[]
                {
                    new TypesForVersion
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleRoom.Background), "房间背景"),
                            (typeof(UndertaleRoom.Tile), "房间图块")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleRoom.Background), null),
                            (typeof(UndertaleRoom.Tile), null),
                            (typeof(UndertaleRoom.Layer), "房间图块图层")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 2, 1),
                        Types = new[]
                        {
                            (typeof(UndertaleTextureGroupInfo), "纹理组")
                        }
                    }
                }
            },
            {
                typeof(UndertaleEmbeddedTexture),
                new[]
                {
                    new TypesForVersion
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleTexturePageItem), "纹理页项")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 2, 1),
                        Types = new[]
                        {
                            (typeof(UndertaleTextureGroupInfo), "纹理组")
                        }
                    },
                }
            },
            {
                typeof(UndertaleFont),
                new[]
                {
                    new TypesForVersion
                    {
                        Version = (2, 2, 1),
                        Types = new[]
                        {
                            (typeof(UndertaleTextureGroupInfo), "纹理组")
                        }
                    }
                }
            },
            {
                typeof(UndertaleTexturePageItem),
                new[]
                {
                    new TypesForVersion()
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleSprite), "精灵"),
                            (typeof(UndertaleBackground), "背景"),
                            (typeof(UndertaleFont), "字体")
                        }
                    },
                    new TypesForVersion()
                    {
                        Version = (2, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleBackground), "图块集"),
                            (typeof(UndertaleEmbeddedImage), "内嵌图像")
                        }
                    }
                }
            },
            {
                typeof(UndertaleString),
                new[]
                {
                    new TypesForVersion
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleBackground), "背景"),
                            (typeof(UndertaleCode), "代码条目（名称和内容）"),
                            (typeof(UndertaleVariable), "变量"),
                            (typeof(UndertaleFunction), "函数"),
                            (typeof(UndertaleSound), "声音"),
                            (typeof(UndertaleAudioGroup), "音频组"),
                            (typeof(UndertaleSprite), "精灵"),
                            (typeof(UndertaleExtension), "扩展"),
                            (typeof(UndertaleExtensionFile), "扩展文件"),
                            (typeof(UndertaleExtensionOption), "扩展选项"),
                            (typeof(UndertaleExtensionFunction), "扩展函数"),
                            (typeof(UndertaleFont), "字体"),
                            (typeof(UndertaleGameObject), "游戏对象"),
                            (typeof(UndertaleGeneralInfo), "常规信息"),
                            (typeof(UndertaleOptions.Constant), "游戏选项常量"),
                            (typeof(UndertalePath), "路径"),
                            (typeof(UndertaleRoom), "房间"),
                            (typeof(UndertaleScript), "脚本"),
                            (typeof(UndertaleShader), "着色器"),
                            (typeof(UndertaleTimeline), "时间轴")
                        }
                    },
                    new TypesForVersion
                    {
                        // Bytecode version 15
                        Version = (15, uint.MaxValue, uint.MaxValue),
                        BeforeVersion = (2024, 8, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleCodeLocals), "代码局部变量")
                        }
                    },
                    new TypesForVersion
                    {
                        // Bytecode version 16
                        Version = (16, uint.MaxValue, uint.MaxValue),
                        Types = new[]
                        {
                            (typeof(UndertaleLanguage), "语言"),
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleBackground), "图块集"),
                            (typeof(UndertaleEmbeddedImage), "内嵌图像"),
                            (typeof(UndertaleRoom.Layer), "房间图层"),
                            (typeof(UndertaleRoom.SpriteInstance), "房间精灵实例")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 2, 1),
                        Types = new[]
                        {
                            (typeof(UndertaleTextureGroupInfo), "纹理组")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 3, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleAnimationCurve), "动画曲线"),
                            (typeof(UndertaleAnimationCurve.Channel), "动画曲线通道"),
                            (typeof(UndertaleRoom.SequenceInstance), "房间序列实例"),
                            (typeof(UndertaleSequence), "序列"),
                            (typeof(UndertaleSequence.Track), "序列轨道"),
                            (typeof(UndertaleSequence.BroadcastMessage), "序列广播消息"),
                            (typeof(UndertaleSequence.Moment), "序列时刻"),
                            (typeof(UndertaleSequence.StringKeyframes), "序列字符串关键帧")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 3, 6),
                        Types = new[]
                        {
                            (typeof(UndertaleFilterEffect), "滤镜效果")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2022, 1, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleRoom.EffectProperty), "房间效果属性")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2022, 2, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleSequence.TextKeyframes), "序列轨道文本关键帧")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2023, 2, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleParticleSystem), "粒子系统"),
                            (typeof(UndertaleParticleSystemEmitter), "粒子系统发射器"),
                            (typeof(UndertaleRoom.ParticleSystemInstance), "房间粒子系统实例")
                        }
                    }
                }
            },
            {
                typeof(UndertaleGameObject),
                new[]
                {
                    new TypesForVersion
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleGameObject), "游戏对象（子对象）"),
                            (typeof(UndertaleRoom.GameObject), "房间对象实例")
                        }
                    },
                    new TypesForVersion
                    {
                        Version = (2, 3, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleSequence.InstanceKeyframes), "序列实例关键帧")
                        }
                    },
                }
            },
            {
                typeof(UndertaleCode),
                new[]
                {
                    new TypesForVersion()
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleGameObject), "游戏对象"),
                            (typeof(UndertaleRoom), "房间"),
                            (typeof(UndertaleRoom.GameObject), "房间对象实例（创建代码）"),
                            (typeof(UndertaleGlobalInit), "全局初始化与游戏结束脚本"),
                            (typeof(UndertaleScript), "脚本")
                        }
                    },
                    new TypesForVersion()
                    {
                        // Bytecode version 16
                        Version = (16, uint.MaxValue, uint.MaxValue),
                        Types = new[]
                        {
                            (typeof(UndertaleRoom.GameObject), "房间对象实例（创建代码或预创建代码）")
                        }
                    }
                }
            },
            {
                typeof(UndertaleEmbeddedAudio),
                new[]
                {
                    new TypesForVersion()
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleSound), "声音")
                        }
                    }
                }
            },
            {
                typeof(UndertaleAudioGroup),
                new[]
                {
                    new TypesForVersion()
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleSound), "声音")
                        }
                    }
                }
            },
            {
                typeof(UndertaleFunction),
                new[]
                {
                    new TypesForVersion()
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleCode), "代码")
                        }
                    }
                }
            },
            {
                typeof(UndertaleVariable),
                new[]
                {
                    new TypesForVersion()
                    {
                        Version = (1, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleCode), "代码")
                        }
                    }
                }
            },
            {
                typeof(ValueTuple<UndertaleBackground, UndertaleBackground.TileID>),
                new[]
                {
                    new TypesForVersion()
                    {
                        Version = (2, 0, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleRoom.Layer), "房间图块图层")
                        }
                    }
                }
            },
            {
                typeof(UndertaleParticleSystem),
                new[]
                {
                    new TypesForVersion()
                    {
                        Version = (2023, 2, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleRoom.ParticleSystemInstance), "房间粒子系统实例")
                        }
                    }
                }
            },
            {
                typeof(UndertaleParticleSystemEmitter),
                new[]
                {
                    new TypesForVersion()
                    {
                        Version = (2023, 2, 0),
                        Types = new[]
                        {
                            (typeof(UndertaleParticleSystem), "粒子系统"),
                            (typeof(UndertaleParticleSystemEmitter), "粒子系统发射器")
                        }
                    }
                }
            }
        };

        // From `typeMap`
        private static readonly Dictionary<Type, (string, GameVersion)> referenceableTypesOrig = new()
        {
            { typeof(UndertaleSprite), ("精灵", (1, 0, 0)) },
            { typeof(UndertaleBackground), ("背景", (1, 0, 0)) },
            { typeof(UndertaleEmbeddedTexture), ("内嵌纹理", (1, 0, 0)) },
            { typeof(UndertaleFont), ("字体", (1, 0, 0)) },
            { typeof(UndertaleTexturePageItem), ("纹理页项", (1, 0, 0)) },
            { typeof(UndertaleString), ("字符串", (1, 0, 0)) },
            { typeof(UndertaleGameObject), ("游戏对象", (1, 0, 0)) },
            { typeof(UndertaleCode), ("代码条目", (1, 0, 0)) },
            { typeof(UndertaleFunction), ("函数", (1, 0, 0)) },
            { typeof(UndertaleVariable), ("变量", (1, 0, 0)) },
            { typeof(UndertaleEmbeddedAudio), ("内嵌音频", (1, 0, 0)) },
            { typeof(UndertaleAudioGroup), ("音频组", (1, 0, 0)) },
            { typeof(UndertaleParticleSystem), ("粒子系统", (2023, 2, 0)) },
            { typeof(UndertaleParticleSystemEmitter), ("粒子系统发射器", (2023, 2, 0)) }
        };
        private static Dictionary<Type, string> referenceableTypes;
        private static GameVersion currVersion;
        
        public static readonly HashSet<Type> CodeTypes = new()
        {
            typeof(UndertaleCode),
            typeof(UndertaleScript),
            typeof(UndertaleCodeLocals),
            typeof(UndertaleVariable),
            typeof(UndertaleFunction)
        };

        public static (Type, string)[] GetTypeMapForVersion(Type type, UndertaleData data)
        {
            if (!typeMap.TryGetValue(type, out TypesForVersion[] typesForVer))
                return null;

            GameVersion version;
            if (data.GeneralInfo.Branch == UndertaleGeneralInfo.BranchType.LTS2022_0)
                version = (2022, 0, 0);
            else
                version = (data.GeneralInfo.Major, data.GeneralInfo.Minor, data.GeneralInfo.Release);
            byte bytecodeVersion = data.GeneralInfo.BytecodeVersion;

            IEnumerable<(Type, string)> outTypes = Enumerable.Empty<(Type, string)>();
            foreach (var typeForVer in typesForVer)
            {
                bool isAtLeast = false;
                if (typeForVer.Version.Minor == uint.MaxValue)
                    isAtLeast = typeForVer.Version.Major <= bytecodeVersion;
                else
                    isAtLeast = typeForVer.Version.CompareTo(version) <= 0;

                bool isAboveMost = false;
                if (typeForVer.BeforeVersion.Minor == uint.MaxValue)
                    isAboveMost = typeForVer.BeforeVersion.Major <= bytecodeVersion;
                else
                    isAboveMost = typeForVer.BeforeVersion.CompareTo(version) <= 0;

                if (isAtLeast && !isAboveMost)
                    outTypes = typeForVer.Types.UnionBy(outTypes, x => x.Item1);
            }

            if (outTypes == Enumerable.Empty<(Type, string)>())
                return null;

            return outTypes.Where(x => x.Item2 is not null
                                       && !(data.Code is null && CodeTypes.Contains(x.Item1)))
                           .ToArray();
        }

        public static Dictionary<Type, string> GetReferenceableTypes(GameVersion version)
        {
            if (version == currVersion && currVersion != default)
                return referenceableTypes;

            referenceableTypes = referenceableTypesOrig.Where(x => x.Value.Item2.CompareTo(version) <= 0)
                                                       .ToDictionary(x => x.Key, x => x.Value.Item1);
            currVersion = version;

            if (referenceableTypes.Count == 0)
                return referenceableTypes;
            
            referenceableTypes[typeof(UndertaleBackground)] = version.CompareTo((2, 0, 0)) >= 0
                                                              ? "图块集" : "背景";

            return referenceableTypes;
        }

        public static bool IsTypeReferenceable(Type type)
        {
            if (type is null)
                return false;

            return typeMap.ContainsKey(type);
        }
    }
}
