using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using UnityEngine;

public enum BuffType
{

}

public enum StatsType
{
	None,
	/// <summary>
	/// 공격력
	/// </summary>
	Atk = 1,
	/// <summary>
	/// 공격력 증폭
	/// </summary>
	Atk_Buff,
	/// <summary>
	/// 체력
	/// </summary>
	Hp = 10,
	/// <summary>
	/// 체력 증폭
	/// </summary>
	Hp_Buff,

	Hp_Recovery = 20,
	Hp_Recovery_Buff = 21,

	/// <summary>
	/// 치명타
	/// </summary>
	Crits_Chance = 30,

	Crits_Damage = 40,

	Super_Crits_Chance = 50,

	Super_Crits_Damage = 60,

	Atk_Speed = 70,

	Move_Speed = 80,

	Skill_Cooltime = 90,
	Skill_Damage = 100,

	Mob_Damage_Buff = 110,
	Boss_Damage_Buff = 120,

	Evasion = 130,

	Buff_Gain_Gold = 140,
	Buff_Gain_Exp = 150,
	Buff_Gain_Item = 160,

	Final_Damage_Buff = 170,
	Damage_Reduce = 180,

	Knockback_Resist = 190,

	//방어력
	Def = 200,
	//방어력 증폭
	Def_Buff = 201,
}

public enum StatModeType
{
	None = 0,
	/// <summary>
	/// 원본 값에 더하기
	/// </summary>
	Add = 1,
	/// <summary>
	/// 원본 값에 곱하기
	/// </summary>
	Multi = 100,
	/// <summary>
	/// 버프
	/// </summary>
	Buff = 130,

	/// <summary>
	/// 광고 버프
	/// </summary>
	AdsBuff = 150,
	Costume_Buff = 160,
	/// <summary>
	/// 하이퍼 버프 
	/// </summary>
	Hyper = 200,


	SkillBuff = 300,
	/// <summary>
	/// 디버프
	/// </summary>
	SkillDebuff = 500,


	Replace = 1000,
}


public class StatsModifier
{
	public readonly IdleNumber Value;
	public readonly StatModeType Type;
	public readonly int Order;
	public readonly object Source;

	public StatsModifier(IdleNumber value, StatModeType type, int order, object source)
	{
		Value = value;
		Type = type;
		Order = order;
		Source = source;
	}

	public StatsModifier(IdleNumber value, StatModeType type) : this(value, type, (int)type, null) { }
	public StatsModifier(IdleNumber value, StatModeType type, int order) : this(value, type, order, null) { }
	public StatsModifier(IdleNumber value, StatModeType type, object source) : this(value, type, (int)type, source) { }


	public bool RemoveModifier()
	{
		return true;
	}
}

[System.Serializable]
public class Stat : ModifyInfo
{
	[HideInInspector] public string name;

	public StatsType type;
	public string baseValue;

	public IdleNumber MaxValue { get; private set; }

	public override IdleNumber Value
	{
		get
		{
			if (isDirty || BaseValue != lastBaseValue)
			{
				lastBaseValue = BaseValue;
				_value = Calculate();

				isDirty = false;
			}

			return _value;
		}
	}


	public Stat(StatsType type, string value = "0") : base()
	{
		this.type = type;
		baseValue = value;
		RawValue = (IdleNumber)baseValue;
		BaseValue = (IdleNumber)baseValue;

		//var datasheet = DataManager.Get<StatusDataSheet>();
		//if (datasheet != null)
		//{
		//	var data = datasheet.GetData(type);
		//	if (data != null)
		//	{
		//		MaxValue = data.MaxValue();
		//	}
		//	else
		//	{
		//		MaxValue = (IdleNumber)"999.99ZZ";
		//	}
		//}
	}
	public bool IsMax()
	{
		return Value >= MaxValue;
	}
	public override IdleNumber Calculate()
	{
		if (modifiers == null || modifiers.Count == 0)
		{

			return BaseValue;
		}

		IdleNumber finalValue = BaseValue;
		IdleNumber multi = (IdleNumber)0;
		IdleNumber buff = (IdleNumber)0;
		IdleNumber adsbuff = (IdleNumber)0;
		IdleNumber costumeBuff = (IdleNumber)0;
		IdleNumber hyper = (IdleNumber)0;
		List<IdleNumber> debuff = new List<IdleNumber>();

		for (int i = 0; i < modifiers.Count; i++)
		{
			var modifier = modifiers[i];
			if (modifier.Type == StatModeType.Replace)
			{
				finalValue = modifier.Value;
			}
			else if (modifier.Type == StatModeType.Add)
			{
				finalValue += modifier.Value;
			}
			else if (modifier.Type == StatModeType.Multi)
			{
				multi += modifier.Value;
			}
			else if (modifier.Type == StatModeType.Buff)
			{
				buff += modifier.Value;
			}
			else if (modifier.Type == StatModeType.SkillBuff)
			{
				buff += modifier.Value;
			}
			else if (modifier.Type == StatModeType.AdsBuff)
			{
				adsbuff += modifier.Value;
			}
			else if (modifier.Type == StatModeType.Costume_Buff)
			{
				costumeBuff += modifier.Value;
			}
			else if (modifier.Type == StatModeType.Hyper)
			{
				hyper += modifier.Value;
			}
			else if (modifier.Type == StatModeType.SkillDebuff)
			{
				debuff.Add(modifier.Value);
			}
			else
			{
				if (modifier.Value == 0)
				{
					continue;
				}
				finalValue *= 1 + (modifier.Value / 100f);
			}
		}


		IdleNumber buffRatio = (IdleNumber)1;
		IdleNumber adsbuffRatio = (IdleNumber)1;
		IdleNumber hyperRatio = (IdleNumber)1;
		IdleNumber costumeBuffRatio = (IdleNumber)1;
		if (buff > 0)
		{
			buffRatio = (IdleNumber)(1 + (buff / 100f));
		}
		if (adsbuff > 0)
		{
			adsbuffRatio = (IdleNumber)(1 + (adsbuff / 100f));
		}
		if (hyper > 0)
		{
			hyperRatio = (IdleNumber)(1 + (hyper / 100f));
		}
		if (costumeBuff > 0)
		{
			costumeBuffRatio = (IdleNumber)(1 + (costumeBuff / 100f));
		}
		//Debug.Log($"{type} Total Hyper Ratio : {hyperRatio.ToFloatingString()}");
		var first = (finalValue * (1 + (multi / 100f)));

		finalValue = first * buffRatio * adsbuffRatio * costumeBuffRatio* hyperRatio;

		for (int i = 0; i < debuff.Count; i++)
		{
			if (debuff[i] > 0)
			{
				IdleNumber minus = finalValue / 100f * debuff[i];
				finalValue -= minus;
			}
		}
		
		if(finalValue > MaxValue)
		{
			finalValue = MaxValue;
		}
		return finalValue;
	}
	public override void SetDirty()
	{
		isDirty = true;
		if ((baseValue != "0" || baseValue != "") && RawValue == 0)
		{
			RawValue = (IdleNumber)baseValue;
			BaseValue = (IdleNumber)baseValue;
		}
	}
}


[CreateAssetMenu(fileName = "Unit Stats", menuName = "ScriptableObject/Unit Stats", order = 1)]
public class UnitStats
{
	public List<Stat> stats;

	public void CopyTo(UnitStats _stats)
	{
		Stat[] array = new Stat[stats.Count];
		stats.CopyTo(array, 0);
		_stats.stats = new List<Stat>(array);
	}
	public void Load()
	{

	}

	public IdleNumber GetValue(StatsType type)
	{
		var stat = stats.Find(x => x.type == type);
		if (stat == null)
		{
			return (IdleNumber)0;
		}
		return stat.Value;
	}
	public IdleNumber GetBaseValue(StatsType type)
	{
		var stat = stats.Find(x => x.type == type);
		if (stat == null)
		{
			return (IdleNumber)0;
		}
		return stat.RawValue;
	}

	public void AddModifier(StatsType type, StatsModifier modifier)
	{
		GetStat(type)?.AddModifiers(modifier);
	}

	private Stat GetStat(StatsType type)
	{
		if (stats == null)
		{
			stats = new List<Stat>();
		}

		StatsType effectType = type;
		if (type == StatsType.Atk_Buff)
		{
			effectType = StatsType.Atk;
		}

		if (type == StatsType.Hp_Buff)
		{
			effectType = StatsType.Hp;
		}
		if(type == StatsType.Def_Buff)
		{
			effectType = StatsType.Def;
		}
		if(type == StatsType.Hp_Recovery_Buff)
		{
			effectType = StatsType.Hp_Recovery;
		}

		Stat stat = stats.Find(x => x.type == effectType);
		if (stat == null)
		{
			stat = new Stat(effectType);
			stat.name = effectType.ToString();
			stats.Add(stat);
		}

		return stat;
	}

	public void UpdataModifier(StatsType type, StatsModifier modifier)
	{
		GetStat(type)?.UpdateModifiers(modifier);
	}
	public void RemoveAllModifiers(object source)
	{
		for (int i = 0; i < stats.Count; i++)
		{
			bool remove = stats[i].RemoveAllModifiersFromSource(source);
			if (remove)
			{
				Debug.Log($"Remove Modifier {source}");
			}
		}
	}
	public void RemoveAllModifiers(StatModeType type)
	{
		for (int i = 0; i < stats.Count; i++)
		{
			stats[i].RemoveAllModifiersFromSource(type);
		}
	}

	public void RemoveModifier(StatsType type, object source)
	{
		GetStat(type)?.RemoveAllModifiersFromSource(source);
	}
	public void Generate()
	{
		StatsType[] types = (StatsType[])System.Enum.GetValues(typeof(StatsType));
		stats = new List<Stat>();
		stats.Add(new Stat(StatsType.Atk, "10"));
		stats.Add(new Stat(StatsType.Hp, "100" ));
		stats.Add(new Stat(StatsType.Hp_Recovery, "5"));
		stats.Add(new Stat(StatsType.Crits_Chance));
		stats.Add(new Stat(StatsType.Crits_Damage, "120" ));
		stats.Add(new Stat(StatsType.Super_Crits_Chance));
		stats.Add(new Stat(StatsType.Super_Crits_Damage, "120" ));
		stats.Add(new Stat(StatsType.Atk_Speed, "100" ));
		stats.Add(new Stat(StatsType.Move_Speed, "100" ));
		stats.Add(new Stat(StatsType.Skill_Cooltime ));
		stats.Add(new Stat(StatsType.Skill_Damage));
		stats.Add(new Stat(StatsType.Mob_Damage_Buff));
		stats.Add(new Stat(StatsType.Boss_Damage_Buff));
		stats.Add(new Stat(StatsType.Buff_Gain_Gold));
		stats.Add(new Stat(StatsType.Buff_Gain_Exp));
		stats.Add(new Stat(StatsType.Buff_Gain_Item));
		stats.Add(new Stat(StatsType.Final_Damage_Buff));

		UpdateAll();
	}
	//public void Generate(List<ItemStats> _stats)
	//{
	//	stats = new List<Stat>();
	//	for(int i=0; i< _stats.Count; i++)
	//	{
	//		stats.Add(new Stat(_stats[i].type, _stats[i].value) );
	//	}
	//	UpdateAll();
	//}

	public void UpdateAll()
	{
		for (int i = 0; i < stats.Count; i++)
		{
			stats[i].SetDirty();
		}
	}

	public IdleNumber GetTotalPower()
	{
		IdleNumber totalPower = (IdleNumber)0;
		IdleNumber atkPower = GetStat(StatsType.Atk).Value;
		IdleNumber atkSpeed = GetStat(StatsType.Atk_Speed).Value / 1000f;

		IdleNumber averageAtk = atkPower * (1 + atkSpeed);

		IdleNumber critChance = GetStat(StatsType.Crits_Chance).Value;
		IdleNumber critDamage = GetStat(StatsType.Crits_Damage).Value / 100f;
		IdleNumber superCritChance = GetStat(StatsType.Super_Crits_Chance).Value;
		IdleNumber superCritDmg = GetStat(StatsType.Super_Crits_Damage).Value / 100f;

		IdleNumber critResult = averageAtk * critChance * (critDamage);
		IdleNumber superCritResult = critResult * superCritChance * superCritDmg;

		critResult.Check();
		superCritResult.Check();
		totalPower = atkPower + averageAtk + critResult + superCritResult;
		totalPower.Turncate();
		return totalPower;
	}
}
