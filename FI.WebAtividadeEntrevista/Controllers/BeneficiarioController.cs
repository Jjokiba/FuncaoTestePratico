using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class BeneficiarioController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Excluir(long id)
        {
            BoBeneficiario bo = new BoBeneficiario();

            bo.Excluir(id);

            return Json("Cadastro Excluido com sucesso");
        }

        [HttpPost]
        public JsonResult Incluir(BeneficiarioModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();
            
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (bo.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF informado já possui cadastro ativo!");
                }
                
                model.Id = bo.Incluir(new Beneficiario()
                {                    
                    IdCliente = model.IdCliente,
                    Nome = model.Nome,
                    CPF = model.CPF
                });

                Response.StatusCode = 200;
                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(BeneficiarioModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();
       
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (bo.VerificarExistencia(model.CPF, model.Id))
                {
                    Response.StatusCode = 400;
                    return Json("CPF informado já possui cadastro ativo!");
                }

                bo.Alterar(new Beneficiario()
                {
                    Id = model.Id,
                    Nome = model.Nome,
                    IdCliente = model.IdCliente,
                    CPF = model.CPF
                });
                               
                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoBeneficiario bo = new BoBeneficiario();
            Beneficiario cliente = bo.Consultar(id);
            Models.BeneficiarioModel model = null;

            if (cliente != null)
            {
                model = new BeneficiarioModel()
                {
                    Id = cliente.Id,
                    Nome = cliente.Nome,
                    CPF = cliente.CPF
                };

            
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Consultar(long id)
        {
            BoBeneficiario bo = new BoBeneficiario();
            Beneficiario beneficiario = bo.Consultar(id);
            Models.BeneficiarioModel model = null;

            if (beneficiario != null)
            {
                model = new BeneficiarioModel()
                {
                    Id = beneficiario.Id,
                    Nome = beneficiario.Nome,
                    CPF = beneficiario.CPF,
                    IdCliente = beneficiario.IdCliente
                };

                return Json(beneficiario);
            }
            else { 
                Response.StatusCode = 404;
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult BeneficiarioList(int idCliente = 0)
        {
            try
            {
                List<Beneficiario> beneficiarios = new BoBeneficiario().Listar(idCliente);

                //Return result to jTable
                return Json(new { Result = "OK", Records = beneficiarios });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}