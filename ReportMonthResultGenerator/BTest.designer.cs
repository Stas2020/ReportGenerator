﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReportMonthResultGenerator
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Btest")]
	public partial class BTestDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Определения метода расширяемости
    partial void OnCreated();
    #endregion
		
		public BTestDataContext() : 
				base(global::ReportMonthResultGenerator.Properties.Settings.Default.BtestConnectionString1, mappingSource)
		{
			OnCreated();
		}
		
		public BTestDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public BTestDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public BTestDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public BTestDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Продажи> Продажиs
		{
			get
			{
				return this.GetTable<Продажи>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Продажи")]
	public partial class Продажи
	{
		
		private System.Nullable<System.Guid> _Глоб;
		
		private System.Nullable<System.DateTime> _ДатаВремя;
		
		private System.Nullable<int> _НомерКассы;
		
		private System.Nullable<int> _НомерЧека;
		
		private System.Nullable<int> _НомерСтроки;
		
		private System.Nullable<int> _БарКод;
		
		private System.Nullable<int> _Колич;
		
		private System.Nullable<int> _КодВалюты;
		
		private System.Nullable<decimal> _КурсВалюты;
		
		private System.Nullable<int> _НомерОфицианта;
		
		private System.Nullable<int> _НомерКассира;
		
		private string _КассовоеМесто;
		
		private System.Nullable<int> _НомерКлиента;
		
		private System.Nullable<int> _ВидОплаты;
		
		private System.Nullable<int> _Стол;
		
		private System.Nullable<int> _ВидСкидки;
		
		private System.Nullable<decimal> _Скидка;
		
		private System.Nullable<decimal> _СуммаИтог;
		
		private System.Nullable<decimal> _СуммаИтогРуб;
		
		private System.Nullable<decimal> _СуммаИтогДолл;
		
		private System.Nullable<decimal> _СуммаИтогЕвро;
		
		private System.Nullable<decimal> _ПоКредКарте;
		
		private System.Nullable<int> _БарКод1;
		
		private System.Nullable<int> _БарКод2;
		
		private System.Nullable<int> _БарКод3;
		
		private System.Nullable<int> _БарКод4;
		
		private System.Nullable<int> _БарКод5;
		
		private System.Nullable<int> _БарКод6;
		
		private System.Nullable<int> _БарКод7;
		
		private System.Nullable<int> _БарКод8;
		
		private System.Nullable<int> _БарКод9;
		
		private System.Nullable<int> _БарКод10;
		
		private System.Nullable<int> _Год;
		
		private System.Nullable<int> _Месяц;
		
		private System.Nullable<int> _Число;
		
		private System.Nullable<int> _Час;
		
		private System.Nullable<int> _ДеньНедели;
		
		private System.Nullable<int> _АбсНомерСтроки;
		
		private System.Nullable<int> _АбсНомерЧека;
		
		private System.Nullable<int> _СквознойНомерЧека;
		
		private string _СквНомСтроки;
		
		private System.Nullable<decimal> _ДоляЛокКода;
		
		private System.Nullable<decimal> _СебестоимЛок;
		
		private System.Nullable<int> _Локкод;
		
		private System.Nullable<decimal> _СебестоимБар;
		
		private System.Nullable<int> _КодПодразд;
		
		private System.Nullable<int> _Терминал;
		
		public Продажи()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Глоб", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> Глоб
		{
			get
			{
				return this._Глоб;
			}
			set
			{
				if ((this._Глоб != value))
				{
					this._Глоб = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ДатаВремя", DbType="DateTime")]
		public System.Nullable<System.DateTime> ДатаВремя
		{
			get
			{
				return this._ДатаВремя;
			}
			set
			{
				if ((this._ДатаВремя != value))
				{
					this._ДатаВремя = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_НомерКассы", DbType="Int")]
		public System.Nullable<int> НомерКассы
		{
			get
			{
				return this._НомерКассы;
			}
			set
			{
				if ((this._НомерКассы != value))
				{
					this._НомерКассы = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_НомерЧека", DbType="Int")]
		public System.Nullable<int> НомерЧека
		{
			get
			{
				return this._НомерЧека;
			}
			set
			{
				if ((this._НомерЧека != value))
				{
					this._НомерЧека = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_НомерСтроки", DbType="Int")]
		public System.Nullable<int> НомерСтроки
		{
			get
			{
				return this._НомерСтроки;
			}
			set
			{
				if ((this._НомерСтроки != value))
				{
					this._НомерСтроки = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод", DbType="Int")]
		public System.Nullable<int> БарКод
		{
			get
			{
				return this._БарКод;
			}
			set
			{
				if ((this._БарКод != value))
				{
					this._БарКод = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Колич", DbType="Int")]
		public System.Nullable<int> Колич
		{
			get
			{
				return this._Колич;
			}
			set
			{
				if ((this._Колич != value))
				{
					this._Колич = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_КодВалюты", DbType="Int")]
		public System.Nullable<int> КодВалюты
		{
			get
			{
				return this._КодВалюты;
			}
			set
			{
				if ((this._КодВалюты != value))
				{
					this._КодВалюты = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_КурсВалюты", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> КурсВалюты
		{
			get
			{
				return this._КурсВалюты;
			}
			set
			{
				if ((this._КурсВалюты != value))
				{
					this._КурсВалюты = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_НомерОфицианта", DbType="Int")]
		public System.Nullable<int> НомерОфицианта
		{
			get
			{
				return this._НомерОфицианта;
			}
			set
			{
				if ((this._НомерОфицианта != value))
				{
					this._НомерОфицианта = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_НомерКассира", DbType="Int")]
		public System.Nullable<int> НомерКассира
		{
			get
			{
				return this._НомерКассира;
			}
			set
			{
				if ((this._НомерКассира != value))
				{
					this._НомерКассира = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_КассовоеМесто", DbType="Char(10)")]
		public string КассовоеМесто
		{
			get
			{
				return this._КассовоеМесто;
			}
			set
			{
				if ((this._КассовоеМесто != value))
				{
					this._КассовоеМесто = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_НомерКлиента", DbType="Int")]
		public System.Nullable<int> НомерКлиента
		{
			get
			{
				return this._НомерКлиента;
			}
			set
			{
				if ((this._НомерКлиента != value))
				{
					this._НомерКлиента = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ВидОплаты", DbType="Int")]
		public System.Nullable<int> ВидОплаты
		{
			get
			{
				return this._ВидОплаты;
			}
			set
			{
				if ((this._ВидОплаты != value))
				{
					this._ВидОплаты = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Стол", DbType="Int")]
		public System.Nullable<int> Стол
		{
			get
			{
				return this._Стол;
			}
			set
			{
				if ((this._Стол != value))
				{
					this._Стол = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ВидСкидки", DbType="Int")]
		public System.Nullable<int> ВидСкидки
		{
			get
			{
				return this._ВидСкидки;
			}
			set
			{
				if ((this._ВидСкидки != value))
				{
					this._ВидСкидки = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Скидка", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> Скидка
		{
			get
			{
				return this._Скидка;
			}
			set
			{
				if ((this._Скидка != value))
				{
					this._Скидка = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_СуммаИтог", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> СуммаИтог
		{
			get
			{
				return this._СуммаИтог;
			}
			set
			{
				if ((this._СуммаИтог != value))
				{
					this._СуммаИтог = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_СуммаИтогРуб", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> СуммаИтогРуб
		{
			get
			{
				return this._СуммаИтогРуб;
			}
			set
			{
				if ((this._СуммаИтогРуб != value))
				{
					this._СуммаИтогРуб = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_СуммаИтогДолл", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> СуммаИтогДолл
		{
			get
			{
				return this._СуммаИтогДолл;
			}
			set
			{
				if ((this._СуммаИтогДолл != value))
				{
					this._СуммаИтогДолл = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_СуммаИтогЕвро", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> СуммаИтогЕвро
		{
			get
			{
				return this._СуммаИтогЕвро;
			}
			set
			{
				if ((this._СуммаИтогЕвро != value))
				{
					this._СуммаИтогЕвро = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ПоКредКарте", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> ПоКредКарте
		{
			get
			{
				return this._ПоКредКарте;
			}
			set
			{
				if ((this._ПоКредКарте != value))
				{
					this._ПоКредКарте = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод1", DbType="Int")]
		public System.Nullable<int> БарКод1
		{
			get
			{
				return this._БарКод1;
			}
			set
			{
				if ((this._БарКод1 != value))
				{
					this._БарКод1 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод2", DbType="Int")]
		public System.Nullable<int> БарКод2
		{
			get
			{
				return this._БарКод2;
			}
			set
			{
				if ((this._БарКод2 != value))
				{
					this._БарКод2 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод3", DbType="Int")]
		public System.Nullable<int> БарКод3
		{
			get
			{
				return this._БарКод3;
			}
			set
			{
				if ((this._БарКод3 != value))
				{
					this._БарКод3 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод4", DbType="Int")]
		public System.Nullable<int> БарКод4
		{
			get
			{
				return this._БарКод4;
			}
			set
			{
				if ((this._БарКод4 != value))
				{
					this._БарКод4 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод5", DbType="Int")]
		public System.Nullable<int> БарКод5
		{
			get
			{
				return this._БарКод5;
			}
			set
			{
				if ((this._БарКод5 != value))
				{
					this._БарКод5 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод6", DbType="Int")]
		public System.Nullable<int> БарКод6
		{
			get
			{
				return this._БарКод6;
			}
			set
			{
				if ((this._БарКод6 != value))
				{
					this._БарКод6 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод7", DbType="Int")]
		public System.Nullable<int> БарКод7
		{
			get
			{
				return this._БарКод7;
			}
			set
			{
				if ((this._БарКод7 != value))
				{
					this._БарКод7 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод8", DbType="Int")]
		public System.Nullable<int> БарКод8
		{
			get
			{
				return this._БарКод8;
			}
			set
			{
				if ((this._БарКод8 != value))
				{
					this._БарКод8 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод9", DbType="Int")]
		public System.Nullable<int> БарКод9
		{
			get
			{
				return this._БарКод9;
			}
			set
			{
				if ((this._БарКод9 != value))
				{
					this._БарКод9 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_БарКод10", DbType="Int")]
		public System.Nullable<int> БарКод10
		{
			get
			{
				return this._БарКод10;
			}
			set
			{
				if ((this._БарКод10 != value))
				{
					this._БарКод10 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Год", DbType="Int")]
		public System.Nullable<int> Год
		{
			get
			{
				return this._Год;
			}
			set
			{
				if ((this._Год != value))
				{
					this._Год = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Месяц", DbType="Int")]
		public System.Nullable<int> Месяц
		{
			get
			{
				return this._Месяц;
			}
			set
			{
				if ((this._Месяц != value))
				{
					this._Месяц = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Число", DbType="Int")]
		public System.Nullable<int> Число
		{
			get
			{
				return this._Число;
			}
			set
			{
				if ((this._Число != value))
				{
					this._Число = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Час", DbType="Int")]
		public System.Nullable<int> Час
		{
			get
			{
				return this._Час;
			}
			set
			{
				if ((this._Час != value))
				{
					this._Час = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ДеньНедели", DbType="Int")]
		public System.Nullable<int> ДеньНедели
		{
			get
			{
				return this._ДеньНедели;
			}
			set
			{
				if ((this._ДеньНедели != value))
				{
					this._ДеньНедели = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_АбсНомерСтроки", DbType="Int")]
		public System.Nullable<int> АбсНомерСтроки
		{
			get
			{
				return this._АбсНомерСтроки;
			}
			set
			{
				if ((this._АбсНомерСтроки != value))
				{
					this._АбсНомерСтроки = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_АбсНомерЧека", DbType="Int")]
		public System.Nullable<int> АбсНомерЧека
		{
			get
			{
				return this._АбсНомерЧека;
			}
			set
			{
				if ((this._АбсНомерЧека != value))
				{
					this._АбсНомерЧека = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_СквознойНомерЧека", DbType="Int")]
		public System.Nullable<int> СквознойНомерЧека
		{
			get
			{
				return this._СквознойНомерЧека;
			}
			set
			{
				if ((this._СквознойНомерЧека != value))
				{
					this._СквознойНомерЧека = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_СквНомСтроки", DbType="Char(20)")]
		public string СквНомСтроки
		{
			get
			{
				return this._СквНомСтроки;
			}
			set
			{
				if ((this._СквНомСтроки != value))
				{
					this._СквНомСтроки = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ДоляЛокКода", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> ДоляЛокКода
		{
			get
			{
				return this._ДоляЛокКода;
			}
			set
			{
				if ((this._ДоляЛокКода != value))
				{
					this._ДоляЛокКода = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_СебестоимЛок", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> СебестоимЛок
		{
			get
			{
				return this._СебестоимЛок;
			}
			set
			{
				if ((this._СебестоимЛок != value))
				{
					this._СебестоимЛок = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Локкод", DbType="Int")]
		public System.Nullable<int> Локкод
		{
			get
			{
				return this._Локкод;
			}
			set
			{
				if ((this._Локкод != value))
				{
					this._Локкод = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_СебестоимБар", DbType="Decimal(18,2)")]
		public System.Nullable<decimal> СебестоимБар
		{
			get
			{
				return this._СебестоимБар;
			}
			set
			{
				if ((this._СебестоимБар != value))
				{
					this._СебестоимБар = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_КодПодразд", DbType="Int")]
		public System.Nullable<int> КодПодразд
		{
			get
			{
				return this._КодПодразд;
			}
			set
			{
				if ((this._КодПодразд != value))
				{
					this._КодПодразд = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Терминал", DbType="Int")]
		public System.Nullable<int> Терминал
		{
			get
			{
				return this._Терминал;
			}
			set
			{
				if ((this._Терминал != value))
				{
					this._Терминал = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
