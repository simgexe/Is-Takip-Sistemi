using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IsTakipSistemi.Models;

namespace IsTakipSistemi.Controllers
{
    public class YoneticiController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: Yonetici
        public ActionResult Index()
        {   //yetki olmadan başka indexlere geçilmesin diye
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if(yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;

                return View();
            }
            else { 
            return RedirectToAction("Index","Login");
            }
        }
        public ActionResult Ata()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                
                var calisanlar = (from p in entity.Personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 2 select p).ToList();

                ViewBag.personeller = calisanlar;
               
                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();
                
                ViewBag.birimAd = birim.birimAd; 
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Ata(FormCollection formCollection )
        {
            string isBaslik = formCollection["isBaslik"];
            string isAciklama = formCollection["isAciklama"];
            int secilenPersonelId =Convert.ToInt32( formCollection["selectPer"]); 
            
            Isler yeniIs = new Isler();
             
            yeniIs.isBaslik = isBaslik;
            yeniIs.isAciklama = isAciklama;
            yeniIs.isPersonelId = secilenPersonelId;
            yeniIs.iletilenTarih = DateTime.Now;
            yeniIs.isDurumId = 1;
            yeniIs.isOkunma = false;

            entity.Isler.Add(yeniIs);
            entity.SaveChanges();
            
            return RedirectToAction("Takip","Yonetici");
        }
        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calisanlar = (from p in entity.Personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 2 select p).ToList();

                ViewBag.personeller = calisanlar;
                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();
               
                ViewBag.birimAd = birim.birimAd;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Takip(int selectPer)
        {
            var secilenPersonel = (from p in entity.Personeller where p.personelId == selectPer select p).FirstOrDefault();

            TempData["secilen"] = secilenPersonel;
            return RedirectToAction("Listele", "Yonetici");
        }
        [HttpGet] 
        public ActionResult Listele()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if(yetkiTurId == 1)
            { 
                Personeller secilenPersonel = (Personeller)TempData["secilen"];

                try
                {

                    //secilen personelin idsine göre işlerini listeleme
                    var isler = (from i in entity.Isler 
                                 where i.isPersonelId == secilenPersonel.personelId 
                                 select i).ToList().OrderByDescending(i => i.iletilenTarih);
                   
                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();

                    return View();
                }
                catch(Exception)
                {
                    return RedirectToAction("Takip", "Yonetici");
                }

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            
        }
    }
}