﻿using AttributeRouting;
using AttributeRouting.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataTables;
using chevron.Models;
using System.Globalization;
using AttributeRouting.Helpers;

namespace chevron.Controllers
{
    [RouteArea("api")]
    public class HomeController : Controller
    {
        chevron.Properties.Settings setting = chevron.Properties.Settings.Default;

        Connection con = new Connection();

        public ActionResult Index()
        {
            ViewBag.daily_vessel = getListVessel();
            //ViewBag.daily_activity = getListActivity();
            ViewBag.daily_unit = getUserUnit();
            ViewBag.daily_unitid = getUserUnitId();
            return View();
        }

        /// <summary>
        /// Dropdown
        /// </summary>

        //private List<SelectListItem> getListAfe()
        //{
        //    List<SelectListItem> afe = new List<SelectListItem>();

        //    con.select("unit_table", "id,afe,name");
        //    while (con.result.Read())
        //    {
        //        afe.Add(new SelectListItem
        //        {
        //            Text = con.result["afe"].ToString(),
        //            Value = con.result["id"].ToString()
        //        });
        //    }
        //    con.Close();

        //    var AfeSorted = (from li in afe orderby li.Text select li).ToList();
        //    return AfeSorted;
        //}
        private List<SelectListItem> getListVessel()
        {
            List<SelectListItem> vessel = new List<SelectListItem>();

            con.select("vessel_table", "name");
            while (con.result.Read())
            {
                vessel.Add(new SelectListItem
                {
                    Text = con.result["name"].ToString(),
                    Value = con.result["name"].ToString()
                });
            }
            con.Close();

            var VesselSorted = (from li in vessel orderby li.Text select li).ToList();

            return VesselSorted;
        }

        //private List<SelectListItem> getListActivity()
        //{
        //    List<SelectListItem> activity = new List<SelectListItem>();

        //    con.select("activity_table", "name");
        //    while (con.result.Read())
        //    {
        //        activity.Add(new SelectListItem
        //        {
        //            Text = con.result["name"].ToString(),
        //            Value = con.result["name"].ToString()
        //        });
        //    }
        //    con.Close();

        //    var ActivitySorted = (from li in activity orderby li.Text select li).ToList();

        //    return ActivitySorted;
        //}

        private List<SelectListItem> getUserUnit()
        {
            List<SelectListItem> unit = new List<SelectListItem>();

            con.select("unit_table", "id, name");
            while (con.result.Read())
            {
                unit.Add(new SelectListItem
                {
                    Text = con.result["name"].ToString(),
                    Value = con.result["name"].ToString()
                });
            }
            con.Close();

            var unitSorted = (from li in unit orderby li.Text select li).ToList();

            return unitSorted;
        }

        private List<SelectListItem> getUserUnitId()
        {
            List<SelectListItem> unit_id = new List<SelectListItem>();

            con.select("unit_table", "id, name");
            while (con.result.Read())
            {
                unit_id.Add(new SelectListItem
                {
                    Text = con.result["name"].ToString(),
                    Value = con.result["id"].ToString()
                });
            }
            con.Close();

            var unitidSorted = (from li in unit_id orderby li.Text select li).ToList();

            return unitidSorted;
        }

        /// <summary>
        /// END DROPDOWn
        /// </summary>
        /// <returns></returns>


        [Route("daily")]
        public ActionResult _ApiDaily()
        {
            var request = System.Web.HttpContext.Current.Request;
            using (var db = new Database(setting.DbType, setting.DbConnection))
            {
                //var response = new Editor(db, "daily_activity")
                var response = new Editor(db, "temp_daily")
                .Model<TempDailyModel>()
                .Where("user_log", Session["userid"], "=")
                .Where("date_input", DateTime.Today.ToString("yyyy-MM-dd"), "=")
                //.Field(new Field("duration").Validator(Validation.Numeric()))
                //.Field(new Field("tgl")
                //    .GetFormatter(Format.DateTime("dd/MM/yyyy H:m:s", "MM/dd/yyyy"))
                //    .Validator(Validation.NotEmpty())
                //)
                .Process(request)
                .Data();
                return Json(response);
            }
        }

        [Route("daily_log")]
        public ActionResult _LogDaily()
        {
            var request = System.Web.HttpContext.Current.Request;
            using (var db = new Database(setting.DbType, setting.DbConnection))
            {
                var response = new Editor(db, "daily_table")
                    .Model<DailyTableModel>()
                    .Where("date_input", DateTime.Today.ToString("yyyy-MM-dd"), "=")
                    .Where("user_log", Session["userid"], "=")
                    .Field(new Field("tgl")
                        .GetFormatter(Format.DateTime("MM/dd/yyyy H:m:s", "MM/dd/yyyy"))
                        .Validator(Validation.NotEmpty())
                    )
                    .Field(new Field("date_input")
                        .GetFormatter(Format.DateTime("MM/dd/yyyy H:m:s", "MM/dd/yyyy"))
                        .Validator(Validation.NotEmpty())
                    )
                    .Process(request)
                    .Data();
                return Json(response);
            }

        }

        [Route("monthly")]
        public ActionResult _ApiMonthly()
        {
            var request = System.Web.HttpContext.Current.Request;
            using (var db = new Database(setting.DbType, setting.DbConnection))
            {
                var response = new Editor(db, "monthly_activity")
                .Model<MonthlyActivityModel>()
                .Field(new Field("tgl")
                    .GetFormatter(Format.DateTime("MM/dd/yyyy H:m:s", "MM/dd/yyyy"))
                    .Validator(Validation.NotEmpty())
                )
                .Field(new Field("duration").Validator(Validation.Numeric()))
                .Process(request)
                .Data();

                return Json(response);
            }
        }

        [Route("dataa")]
        public ActionResult _dataActivity()
        {
            using (var db = new Database(setting.DbType, setting.DbConnection))
            {
                var response = new Editor(db, "activity_table")
                .Model<ActivityModel>()
                .Process(Request.Form)
                .Data();

                return Json(response);
            }
        }

        [Route("datau")]
        public ActionResult _dataUnit()
        {
            using (var db = new Database(setting.DbType, setting.DbConnection))
            {
                var response = new Editor(db, "unit_table")
                .Model<UnitDistanceDailyModel>()
                .Process(Request.Form)
                .Data();

                return Json(response);
            }
        }

        [Route("datav")]
        public ActionResult _dataVessel()
        {
            using (var db = new Database(setting.DbType, setting.DbConnection))
            {
                var response = new Editor(db, "vessel_table")
                .Model<VesselModel>()
                .Process(Request.Form)
                .Data();

                return Json(response);
            }
        }

        [Route("drill/{tg1}/{tg2}/{unitx}")]
        public ActionResult _dataDrillCompletion(string tg1, string tg2, int unitx)
        {

            //var un = (unitx === "all")? 
            var request = System.Web.HttpContext.Current.Request;
            using (var db = new Database(setting.DbType, setting.DbConnection))
            {
                if (unitx > 0)
                {
                    var response = new Editor(db, "drilling_table")
                        .Model<DrillModel>()
                        .Field(new Field("drilling_table.tgl")
                            .GetFormatter(Format.DateTime("MM/dd/yyyy H:m:s", "MM/dd/yyyy"))
                            .Validator(Validation.NotEmpty())
                        )
                        .Field(new Field("drilling_table.t_start")
                            .GetFormatter(Format.DateTime("MM/dd/yyyy H:m:s", "H:m"))
                            .Validator(Validation.NotEmpty())
                        )
                        .Field(new Field("drilling_table.t_end")
                            .GetFormatter(Format.DateTime("MM/dd/yyyy H:m:s", "H:m"))
                            .Validator(Validation.NotEmpty())
                        )
                        .LeftJoin("unit_table", "unit_table.id", "=", "drilling_table.id_unit")
                         //.LeftJoin("mainunit_table","")
                         .Where("drilling_table.tgl", tg1, ">")
                         .Where("drilling_table.tgl", tg2, "<=")
                         .Where("drilling_table.id_unit", unitx, "=")
                        .Process(request).Data();
                    return Json(response);
                }
                else
                {
                    var response = new Editor(db, "drilling_table")
                        .Model<DrillModel>()
                        .Field(new Field("drilling_table.tgl")
                            .GetFormatter(Format.DateTime("MM/dd/yyyy H:m:s", "MM/dd/yyyy"))
                            .Validator(Validation.NotEmpty())
                        )
                        .Field(new Field("drilling_table.t_start")
                            .GetFormatter(Format.DateTime("MM/dd/yyyy H:m:s", "H:m"))
                            .Validator(Validation.NotEmpty())
                        )
                        .Field(new Field("drilling_table.t_end")
                            .GetFormatter(Format.DateTime("MM/dd/yyyy H:m:s", "H:m"))
                            .Validator(Validation.NotEmpty())
                        )
                        .LeftJoin("unit_table", "unit_table.id", "=", "drilling_table.id_unit")
                        //.LeftJoin("mainunit_table","")
                        .Where("drilling_table.tgl", tg1, ">")
                        .Where("drilling_table.tgl", tg2, "<=")

                        .Process(request).Data();

                    //Response.Write(unitx);
                    return Json(response);
                }

                //var response1 =  new Editor(db.Select("drilling_table",))


            }
            //return Json(new { date = tg1, date2 = tg2, unitnya = unitx },
            //     JsonRequestBehavior.AllowGet);
        }

        //[Route("drill/{tg1}/{tg2}/{unit}")]
        //public ActionResult _dataDrillCompletionParam(string tg1, string tg2,string unit)
        //{

        //    //Response.Write
        //    return Json(new { date = tg1, date2 = tg2, unitnya = unit },
        //            JsonRequestBehavior.AllowGet);
        //}

        [Route("cs/daily")]
        [HttpPost]
        public String _dailyInsert(FormCollection input)
        {
            String query = "";
            switch (input["action"])
            {
                case "create":
                    var val_unit = string.Format("date_input = '{0}' and user_log = '{1}' and user_unit = '{2}'", DateTime.Today.ToString("yyyy-MM-dd"),Session["userid"],input["daily_unit"]);
                    con.select("temp_daily", "user_unit", val_unit);
                    con.result.Read();
                    if (con.result.HasRows)
                    {
                        query = string.Format("update temp_daily set [duration] = {0} where date_input = CAST('{1}' as DATE) and user_log = '{2}' and user_unit = '{3}'"
                                ,input["time_at_dur"].ToString(CultureInfo.InvariantCulture), DateTime.Today.ToString("yyyy-MM-dd"), Session["userid"], input["daily_unit"]);
                    }
                    else
                    {
                        query = string.Format("insert into temp_daily ([user_unit],[duration],[user_log],[date_input]) values ('{0}',{1},'{2}',CAST('{3}' as DATE ))"
                                ,input["daily_unit"], input["time_at_dur"].ToString(CultureInfo.InvariantCulture), Session["userid"], DateTime.Today.ToString("yyyy-MM-dd"));
                    }
                    break;
                case "update":
                    query = string.Format("update temp_daily set user_unit = '{0}', duration = {1} where user_log = '{2}' and date_input = '{3}' and id = {4}",
                            input["daily_unit"], input["time_at_dur"].ToString(CultureInfo.InvariantCulture), Session["userid"], DateTime.Today.ToString("yyyy-MM-dd"), input["id"]);
                    break;
                default:
                    break;
            }
            try
            {
                //Response.Write(query);
                con.queryExec(query);
                return "success";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            finally
            {
                con.Close();
            }
        }

        [Route("save/daily")]
        [HttpPost]
        public void _saveDailyData(FormCollection input)
        {
            //String qdailytable = "";

            Response.Write("isi dari mob : " +input["mob"]);
            

            var stb     = (input["standby"] == "") ? Convert.ToInt16(0) :  Convert.ToDecimal(input["standby"]);
            var load    = (input["load"] == "") ? Convert.ToInt16(0) : Convert.ToDecimal(input["load"]);
            var steam   = (input["steaming"] == "") ? Convert.ToInt16(0) : Convert.ToDecimal(input["steaming"]);
            var down    = (input["downtime"] == "") ? Convert.ToInt16(0) : Convert.ToDecimal(input["downtime"]);
            var fuel    = (input["daily_fuel"] == "") ? Convert.ToInt16(0) : Convert.ToInt32(input["daily_fuel"]);
            var tanggal = (input["daily_date"] == "") ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") : Convert.ToDateTime(input["daily_date"]).ToString("yyyy-MM-dd");
            //var tanggal = (input["daily_date"] == "") ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") :  DateTime.ParseExact(input["daily_date"], "yyyy-MM-dd", null).ToString();

            //Response.Write(tanggal);

            //buat variabel harga fuel
            decimal harga,curr_harga;
            var cari_fuel = string.Format("tgl = '{0}'",tanggal);
            con.select("fuel_table", "cost_usd, currency_type", cari_fuel);
            con.result.Read();
            if (con.result.HasRows)
            {
                harga = Convert.ToDecimal(con.result["cost_usd"]);
                curr_harga = Convert.ToDecimal(con.result["currency_type"]);
            }
            else
            {
                harga = 0;
                curr_harga = 0;
            }
            con.Close();

            //buat variable charter_rate
            //select* from hire_table where tgl_start <= '2016-01-14' and tgl_end >= '2016-01-14' and vessel = 'Vessel A'
            decimal charter_rate, mob_demob_rate, curr_charter, mob_cost, period;
            var cari_charter = string.Format("tgl_start <= '{0}' and tgl_end >= '{0}' and vessel = '{1}'", tanggal, input["daily_vessel"]);
            con.select("hire_table", "cost_usd, mob_cost,curency_cat, mob_rate, periode", cari_charter);
            con.result.Read();
            if(con.result.HasRows)
            {
                charter_rate    = Convert.ToDecimal(con.result["cost_usd"]);
                
                mob_cost        = (input["mob"] == "" )?  0 : Convert.ToDecimal(con.result["mob_cost"]) ;
                mob_demob_rate  = (input["mob"] == "") ? 0 : Convert.ToDecimal(con.result["mob_rate"]);
                curr_charter    = Convert.ToDecimal(con.result["curency_cat"]);
                period          = Convert.ToDecimal(con.result["periode"]);
            }
            else {
                charter_rate = 0; mob_cost = 0; mob_demob_rate = 0; period = 0; curr_charter = 0;
            }
            con.Close();


            List<DailyUnitActivityModel> usernit = new List<DailyUnitActivityModel>();
            var skr = DateTime.Now.ToString("yyyy-MM-dd");
            var where = string.Format("user_log='{0}' and date_input='{1}'", Session["userid"], skr);
            con.select("temp_daily", "count(user_unit) as jml", where);
            con.result.Read();
            var jml_unit = Convert.ToInt16(con.result["jml"]);
            con.Close();
            //Response.Write(jml_unit);

            // query jadi berubah ----
            
            var qunit = string.Format("select td.user_unit,td.duration,ut.distance from temp_daily td "+
                                            "inner join unit_table ut on ut.name = td.user_unit "+
                                            "where td.user_log = '{0}' and td.date_input = '{1}'",Session["userid"],skr);

            Response.Write("mob_cost: " + mob_cost + ", mob_rate: " + mob_demob_rate+"; ");
            Response.Write(qunit);

            /*
            con.query(qunit);
            while (con.result.Read())
            {
                usernit.Add(new DailyUnitActivityModel
                {
                    user_unit = con.result["user_unit"].ToString(),
                    durasi = (decimal)con.result["duration"],
                    jarak = Convert.ToInt32(con.result["distance"]),
                    hit = Convert.ToInt16(1)
                });
            }
            con.Close();
            */
            // ------------
            //buat variabel total jarak, dan jumlah user_unit
            decimal total_jarak = 0, tot_hit = 0;
            foreach (DailyUnitActivityModel unit_jar in usernit) {
                total_jarak += unit_jar.jarak;
                tot_hit += unit_jar.hit;
            }

            //Response.Write(total_jarak + " total jarak "+tot_hit);
            decimal t_standby = 0,t_load =0,t_steam=0,t_all = 0,fuel_l =0, fuel_price=0,charter_price=0, t_stb_mob=0, t_all_mob = 0,mob_price=0;
            if (fuel > 0)
            {
                foreach (DailyUnitActivityModel unit in usernit)
                {
                    //simpan ke daily_table    
                    var qdailytable = string.Format("insert into daily_table (tgl,vessel,standby,loading,steaming,downtime,user_unit,duration,distance,fuel_tot,user_log,date_input) " +
                        "values (cast('{0}' as DATE),'{1}',{2},{3},{4},{5},'{6}',{7},{8},{9},'{10}','{11}'); "
                        ,input["daily_date"], input["daily_vessel"], stb, load, steam, down, unit.user_unit, unit.durasi, unit.jarak, fuel, Session["userid"], DateTime.Now.ToString("yyyy-MM-dd"));
                    //Response.Write(qdailytable);
                    con.queryExec(qdailytable);

                    t_standby = stb / tot_hit;
                    t_load = load / tot_hit;
                    t_steam = steam * (unit.jarak / total_jarak);
                    t_all = t_standby + t_load + t_steam + unit.durasi;
                    fuel_l = (t_all / (24 - down)) * fuel ;
                    fuel_price = harga * fuel_l;
                    charter_price = t_all * charter_rate / 24;

                    t_stb_mob = stb * (t_load + t_steam + unit.durasi);
                    t_all_mob = t_stb_mob + t_load + t_steam + unit.durasi;
                    mob_price = t_all_mob *(mob_demob_rate/24);
                    

                    var q_rpt1 = string.Format("insert into report_daily (tgl,vessel,user_unit,t_standby,t_load,t_steam,t_down,t_durasi,t_all,fuel_litre,fuel_price,fuel_curr,charter_price,charter_curr,mob_price) values ('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}); ",
                            tanggal,input["daily_vessel"],unit.user_unit,t_standby,t_load,t_steam,down,unit.durasi,t_all,fuel_l,fuel_price,curr_harga,charter_price,curr_charter,mob_price
                        );
                    //Response.Write(q_rpt1);
                    con.queryExec(q_rpt1);

                }
            }



            //delete setelah input
            var qdelete = string.Format("delete temp_daily where date_input = '{0}' and user_log = '{1}'", skr, Session["userid"]);
            //Response.Write(qdelete);

            //con.queryExec(qdelete);
            //return "success";
        }


        [Route("filter/monthly")]
        [HttpPost]
        public ActionResult _MonthlyGrid(FormCollection input)
        {
            List<MonthlyActivityModel> data = new List<MonthlyActivityModel>();
            var tgl_e = input["tgl_e"];
            var tgl_f = input["tgl_eop"];
            var nowDate = DateTime.Today.ToString("MM/dd/yyyy");

            if (string.IsNullOrEmpty(tgl_e) && string.IsNullOrEmpty(tgl_f))
            {
                tgl_e = nowDate;
                tgl_f = nowDate;
            }
            var query = String.Format("select * from monthly_activity where tgl between '{0}' and '{1}'", tgl_e, tgl_f);
            
            con.query(query);
            while (con.result.Read())
            {
                data.Add(new MonthlyActivityModel
                {
                    tgl = DateTime.Parse(con.result["tgl"].ToString()).ToString("MM/dd/yyyy"),
                    activity = con.result["activity"].ToString(),
                    duration = Decimal.Parse(con.result["duration"].ToString()),
                    unit = con.result["unit"].ToString(),
                    vessel = con.result["vessel"].ToString()
                });
            }

            //ViewBag.data = data;
            return PartialView("_MonthlyGrid", data);
        }

        [Route("save/drill")]
        [HttpPost]
        public string _saveDailyDrill(FormCollection input)
        {
            var awal = Convert.ToDateTime(input["t_start"]);
            var akhir = Convert.ToDateTime(input["t_end"]);
            TimeSpan dur = akhir - awal;
            string qq = null;

            if (input["action"] == "create")
            {
                qq = string.Format("insert into drilling_table (id_unit,well,afe,psc_no,tgl,t_start,t_end,durasi) values ({0},'{1}','{2}','{3}','{4}','{5}','{6}',{7})", input["daily_unitid"], input["well"], input["afe"], input["psc"], input["drill_date"], input["t_start"], input["t_end"], dur.TotalHours);

            }
            else if (input["action"] == "update") {
                qq = string.Format("update drilling_table set id_unit={0}, well='{1}', afe = '{2}',psc_no='{3}',tgl = '{4}',t_start = '{5}',t_end='{6}',durasi={7} where id = {8}", input["daily_unitid"], input["well"], input["afe"], input["psc"], input["drill_date"], input["t_start"], input["t_end"], dur.TotalHours,input["id"]);

            }
            Response.Write(input["mob"]);
            //Response.Write(qq);
            //con.queryExec(qq);
            return "success";

        }
    }
}
