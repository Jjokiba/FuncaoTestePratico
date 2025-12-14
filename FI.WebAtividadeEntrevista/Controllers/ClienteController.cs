using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.ComponentModel.DataAnnotations;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
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
            BoCliente bo = new BoCliente();

            bo.Excluir(id);

            return Json("Cadastro Excluido com sucesso");
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

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

                var beneficiarioErrors = new List<string>();
                if (model?.Beneficiarios?.Any() ?? false)
                {
                    foreach (var beneficiario in model.Beneficiarios)
                    {
                        var benefModel = new Models.BeneficiarioModel
                        {
                            IdCliente = 0,
                            Nome = beneficiario.Nome,
                            CPF = beneficiario.CPF
                        };
                        var ctx = new ValidationContext(benefModel, null, null);
                        var results = new List<ValidationResult>();
                        bool isValid = Validator.TryValidateObject(benefModel, ctx, results, true);
                        if (!isValid)
                        {
                            beneficiarioErrors.AddRange(results.Select(r => r.ErrorMessage));
                        }
                        else
                        {
                            // Check existence using BLL (simulate BeneficiarioController logic)
                            BoBeneficiario boBenef = new BoBeneficiario();
                            if (boBenef.VerificarExistencia(benefModel.CPF))
                            {
                                beneficiarioErrors.Add($"CPF {benefModel.CPF} informado já possui cadastro ativo!");
                            }
                        }
                    }
                }

                if (beneficiarioErrors.Any())
                {
                    Response.StatusCode = 400;
                    return Json(string.Join("\n", beneficiarioErrors));
                }

                model.Id = bo.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });
                if (model?.Beneficiarios?.Any() ?? false)
                {
                    BoBeneficiario boBeneficiario = new BoBeneficiario();
                    foreach (var beneficiario in model.Beneficiarios)
                    {
                        boBeneficiario.Incluir(new Beneficiario()
                        {
                            IdCliente = model.Id,
                            Nome = beneficiario.Nome,
                            CPF = beneficiario.CPF
                        });
                    }
                }

                Response.StatusCode = 200;
                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
       
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

                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });
                               
                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };

            
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}