﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MemberShipWebsite.Entities;
using MemberShipWebsite.Models;
using MemberShipWebsite.Areas.Admin;
using MemberShipWebsite.Areas.Admin.Models;
using MemberShipWebsite.Areas.Admin.Extensions;

namespace MemberShipWebsite.Areas.Admin.Controllers
{
    public class SubscriptionProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/SubscriptionProduct
        public async Task<ActionResult> Index()
        {
            return View(await db.SubscriptionProducts.Convert(db));
        }

        // GET: Admin/SubscriptionProduct/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionProduct subscriptionProduct = await db.SubscriptionProducts.FindAsync(id);
            if (subscriptionProduct == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionProduct);
        }

        // GET: Admin/SubscriptionProduct/Create
        public async Task<ActionResult> Create()
        {
            var model = new SubscriptionProductModel
            {
                Products = await db.Products.ToListAsync(),
                Subscriptions = await db.Subscriptions.ToListAsync()
            };
            
            return View(model);
        }

        // POST: Admin/SubscriptionProduct/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductId,SubscriptionId")] SubscriptionProduct subscriptionProduct)
        {
            if (ModelState.IsValid)
            {
                db.SubscriptionProducts.Add(subscriptionProduct);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(subscriptionProduct);
        }

        // GET: Admin/SubscriptionProduct/Edit/5
        public async Task<ActionResult> Edit(int? productId,int? subscriptionId)
        {
            if (subscriptionId == null && productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionProduct subscriptionProduct = await GetSubscriptionProduct(subscriptionId, productId);
            if (subscriptionProduct == null)
            {
                return HttpNotFound();
            }
            return View(await subscriptionProduct.Convert(db));
        }

        // POST: Admin/SubscriptionProduct/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductId,SubscriptionId")] SubscriptionProduct subscriptionProduct)
        {
            if (ModelState.IsValid)
            {
                bool canChange = await subscriptionProduct.CanChange(db);
                if (canChange)
                {
                   await subscriptionProduct.Change(db);
                }
                return RedirectToAction("Index");
            }
            return View(subscriptionProduct);
        }

        // GET: Admin/SubscriptionProduct/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionProduct subscriptionProduct = await db.SubscriptionProducts.FindAsync(id);
            if (subscriptionProduct == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionProduct);
        }

        // POST: Admin/SubscriptionProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SubscriptionProduct subscriptionProduct = await db.SubscriptionProducts.FindAsync(id);
            db.SubscriptionProducts.Remove(subscriptionProduct);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private async Task<SubscriptionProduct> GetSubscriptionProduct(int? subscriptionId, int? productId)
        {
            try
            {
                int subId = 0, prodId = 0;
                int.TryParse(subscriptionId.ToString(), out subId);
                int.TryParse(productId.ToString(), out prodId);
                var subscriptionProduct = await db.SubscriptionProducts.FirstOrDefaultAsync(mi => mi.ProductId.Equals(prodId) && mi.SubscriptionId.Equals(subId));
                return subscriptionProduct;
            }
            catch (Exception)
            {
                return null;
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
