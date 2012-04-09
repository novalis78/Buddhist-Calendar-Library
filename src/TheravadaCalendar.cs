/*
 * Created by SharpDevelop.
 * User: novalis78
 * Date: 25.01.2005
 * Time: 21:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace BuddhistCalendar
{
	/// <summary>
	/// Description of TheravadaCalendar
	/// </summary>
	public class TheravadaCalendar : MoonCalendar
	{
		private const int ERA 	  = 543;
		private int buddhistYear  = 0;
		private int buddhistMonth = 0;
		private int buddhistDay   = 0;
		private int buddhistFortnight = 0;
		private string buddhistDayOfWeek = "";
		private bool abbreviateWeekDay = false;
		private DateTime gregorianDate = DateTime.Now;
		private MoonPhases recentUposatha = MoonPhases.New;
		private MoonPhases upcomingUposatha = MoonPhases.Full;
		public  enum BuddhistWeekDay {Ravivara, Candavara, Kujavara, Budhavara, Guruvara, Sukkavara, Sanivara};
		public  enum BuddhistWeekDayShort {Ra, Ca, Ku, Bu, Gu, Su, Sa};
		public  enum BuddhistFortnights {Kanhapakkha = 1, Sukkapakkha};
		public  enum BuddhistMonths {Phussa = 1, Magha, Phagguna, Citta, Vesakha, Jettha, Asalha, Savana, Potthapada, Assayuja, Kattika, Magasira };
		public  enum FormatStyles { VeryVerbose, Verbose, Short, VeryShort};
		private BuddhistMonths buddhistNewYearMonth = BuddhistMonths.Vesakha;
		
		#region Calendar Constructors
		public TheravadaCalendar(int year, int month, int date, int hour, int minute, int second)
		{
			gregorianDate = new DateTime(year, month, date, hour, minute, second);
			buddhistYear  = getBuddhistYear(gregorianDate);
			buddhistMonth = getBuddhistMonth(gregorianDate);
			buddhistDay   = getBuddhistDay(gregorianDate);
			buddhistFortnight = getBuddhistFortnight(gregorianDate);
			buddhistDayOfWeek = getBuddhistWeekDay(buddhistDay, abbreviateWeekDay);
		}
		
		public TheravadaCalendar(int year, int month, int date)
		{
			DateTime d = new DateTime(year, month, date);
			gregorianDate = d;
			buddhistYear = getBuddhistYear(d);
			buddhistMonth = getBuddhistMonth(d);
			buddhistDay   = getBuddhistDay(d);
			buddhistFortnight = getBuddhistFortnight(d);
			buddhistDayOfWeek = getBuddhistWeekDay(buddhistDay, abbreviateWeekDay);
		}
		
		public TheravadaCalendar(int year)
		{
			DateTime d = DateTime.Now;
			DateTime p = new DateTime(year, d.Month, d.Day);
			gregorianDate = p;
			buddhistYear = getBuddhistYear(p);
			buddhistMonth = getBuddhistMonth(p);
			buddhistDay   = getBuddhistDay(p);
			buddhistFortnight = getBuddhistFortnight(p);
			buddhistDayOfWeek = getBuddhistWeekDay(buddhistDay, abbreviateWeekDay);
		}
		#endregion
		
		#region Buddhist Year
		private int getBuddhistYear(DateTime dt)
		{
			if(belongsToNewBuddhistYear(dt))
				return dt.Year + ERA + 1;
			else
				return dt.Year + ERA;	
		}		
		
		private bool belongsToNewBuddhistYear(DateTime dt)
		{
			DateTime a = getUposathaDateForBuddhistMonth(buddhistNewYearMonth, MoonPhases.Full);
			if(dt <= a)
				return false;
			else
				return true;
		}
		#endregion
		
		#region Buddhist Month	
		private int getBuddhistMonth(DateTime dt)
		{
			ArrayList a = getFullMoonListForYear(dt.Year);
			for(int i = 0; i < a.Count; i++)
			{
				if((DateTime)a[i] > dt)
					return i+1;
				else
					continue;
			}
			return 0;
		}
		#endregion
		
		#region Buddhist Fortnight
		private int getBuddhistFortnight(DateTime dt)
		{
			DateTime a = getMoonPhaseDate(dt.Year, dt.Month, MoonPhases.New);
			DateTime b = getMoonPhaseDate(dt.Year, dt.Month, MoonPhases.Full);
			if(dt <= a && dt < b)
				return 1;
			else if(dt > a && dt <= b)
				return 2;
			else if(dt >= a && dt > b)
				return 1;
			else return 0;
		}
		#endregion
		
		#region Buddhist Day
		private int getBuddhistDay(DateTime dt)
		{
			DateTime a = getMoonPhaseDate(dt.Year, dt.Month, MoonPhases.New);
			DateTime b = getMoonPhaseDate(dt.Year, dt.Month, MoonPhases.Full);
			if(dt < a && dt < b)
			{
				DateTime d = dt.AddMonths(-1);
				DateTime e = getMoonPhaseDate(d.Year, d.Month, MoonPhases.Full);
				if(dt > e)
					return (dt.Day + getRemainingDaysInMonth(e));
				else
					return 0;
			}
			else if(dt > a && dt <= b)
				return (dt.Day - a.Day);
			else if(dt > a && dt > b)
				return (dt.Day - b.Day);
			else return 0;
		}
		#endregion
		
		#region Buddhist Week
		private string getBuddhistWeekDay(int buddhistDay, bool abbreviate)
		{
			int a = buddhistDay % 7;
			if(!abbreviate)
			{
				return ((BuddhistWeekDay)a).ToString();
			}
			else
			{
				return ((BuddhistWeekDayShort)a).ToString();
			}
		}
		#endregion
		
		#region locating Uposatha
		public DateTime getNextUposatha()
		{
			int m = getBuddhistMonth(gregorianDate);
			DateTime tg = DateTime.Now;
			DateTime dt = gregorianDate;
			DateTime a = getNextUposatha(MoonPhases.Full);
			DateTime b = getNextUposatha(MoonPhases.New);
			DateTime c = getNextUposatha(MoonPhases.Waxing);
			DateTime d = getNextUposatha(MoonPhases.Waning);
			if(dt <= d)
				{tg = d; upcomingUposatha = MoonPhases.Waning;}
			else if(dt <= b)
				{tg = b; upcomingUposatha = MoonPhases.New;}
			else if(dt <= c)
				{tg = c; upcomingUposatha = MoonPhases.Waxing;}
			else {tg = a; upcomingUposatha = MoonPhases.Full;}
			return tg;			
		}
		

		
		public DateTime getLastUposatha()
		{
			int m = getBuddhistMonth(gregorianDate);
			DateTime tg = DateTime.Now;
			DateTime dt = gregorianDate;
			DateTime a = getLastUposatha(MoonPhases.Full);
			DateTime b = getLastUposatha(MoonPhases.New);
			DateTime c = getLastUposatha(MoonPhases.Waxing);
			DateTime d = getLastUposatha(MoonPhases.Waning);
			if(dt >= d)
				{tg = b; recentUposatha = MoonPhases.New;}
			else if(dt >= b)
				{tg = c; recentUposatha = MoonPhases.Waxing;}
			else if(dt >= c)
				{tg = a; recentUposatha = MoonPhases.Full;}
			else { tg = d; recentUposatha = MoonPhases.Waning;}
			/*last uposatha is final moon in  last month*/
			if(tg == getNextUposatha())
			{recentUposatha = MoonPhases.Full; return getUposathaDateForBuddhistMonth((BuddhistMonths)(m-1), MoonPhases.Full);}
			else
				return tg;
		}		
		
		private DateTime getNextUposatha(MoonPhases ps)
		{
			int a = getBuddhistMonth(gregorianDate);
			return getUposathaDateForBuddhistMonth((BuddhistMonths)a, ps);
		}
		
		private DateTime getLastUposatha(MoonPhases ps)
		{
			int a = getBuddhistMonth(gregorianDate);
			return getUposathaDateForBuddhistMonth((BuddhistMonths)a, ps);
		}		
		
		public DateTime getUposathaDateForBuddhistMonth(BuddhistMonths bm, MoonPhases ps)
		{
			switch(ps)
			{
					case(MoonPhases.Full): {
						ArrayList a = getFullMoonListForYear(gregorianDate.Year);
				 		return (DateTime)a[((int)bm)-1];
					}
					case(MoonPhases.New):{
						ArrayList a = getNewMoonListForYear(gregorianDate.Year);
						return (DateTime)a[((int)bm)-1];
					}
					case(MoonPhases.Waning):{
						ArrayList a = getWaningMoonListForYear(gregorianDate.Year);
						return (DateTime)a[((int)bm)-1];
					}
					case(MoonPhases.Waxing):{
						ArrayList a = getWaxingMoonListForYear(gregorianDate.Year);
						return (DateTime)a[((int)bm)-1];
					}	
			}
			return DateTime.Now;
		}

		#endregion
		
		#region utilities...
		private int getRemainingDaysInMonth(DateTime a)
		{
			int b = DateTime.DaysInMonth(a.Year, a.Month);
			return (b - a.Day);
		}
		#endregion
		
		#region Formatting Theravada Date
		public static string Format(TheravadaCalendar tc,  FormatStyles styles)
		{//sirisakyamunino tathagataparinibbana
			switch(styles)
			{
				case (FormatStyles.VeryVerbose): return "sirisakyamunino tathagataparinibbana" + tc.BuddhistYear.ToString() + " vasse, " 
					+ ((BuddhistMonths)tc.BuddhistMonth).ToString() + "mase, "
					+ tc.BuddhistDay.ToString() + ". "
					+ ((BuddhistFortnights)tc.BuddhistFortnight).ToString();
				case (FormatStyles.Verbose): return "Sirisugataparinibbana " + tc.BuddhistYear.ToString() + " vasse, " 
					+ ((BuddhistMonths)tc.BuddhistMonth).ToString() + "mase, "
					+ tc.BuddhistDay.ToString() + ". "
					+ ((BuddhistFortnights)tc.BuddhistFortnight).ToString();
				case (FormatStyles.Short): return tc.BuddhistYear.ToString() + "-" + tc.BuddhistMonth.ToString() + "-"
					+ tc.BuddhistFortnight.ToString() + "-" + tc.BuddhistDay.ToString();
				case (FormatStyles.VeryShort): return "";
			}
			return "";
		}
		#endregion
				
		#region Getters and Setters
		public int BuddhistYear {
			get {
				return buddhistYear;
			}
		}

		//Set to true, if you want to abbreviate week days
		public bool AbbreviateWeekDay {
			set {
				abbreviateWeekDay = value;
			}
		}
		
		public string BuddhistDayOfWeek {
			get {
				return buddhistDayOfWeek;
			}
		}
		
		public BuddhistMonths BuddhistNewYearMonth {
			set {
				buddhistNewYearMonth = value;
			}
		}
		public int BuddhistMonth {
			get {
				return buddhistMonth;
			}
		}
		
		public int BuddhistFortnight {
			get {
				return buddhistFortnight;
			}
		}
		
		public int BuddhistDay {
			get {
				return buddhistDay;
			}
		}
		
		public MoonPhases UpcomingUposatha {
			get {
				return upcomingUposatha;
			}
		}
		
		public MoonPhases RecentUposatha {
			get {
				return recentUposatha;
			}
		}
		
		
		#endregion
		
	}
}
