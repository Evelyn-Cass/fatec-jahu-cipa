using CipaFatecJahu.Models;
using CipaFatecJahu.Services;
using CipaFatecJahu.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CipaFatecJahu.Controllers
{
    public class ContactController : Controller
    {
        private readonly EmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContactController(EmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _emailService = emailService;
            _userManager = userManager;
        }

        [Route("Contact")]
        public IActionResult Contact(bool success)
        {
            ViewBag.Success = success;
            return View();
        }

        [HttpPost]
        [Route("Contact")]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var body = $@"
                <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                <html dir=""ltr"" xmlns=""http://www.w3.org/1999/xhtml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" lang=""pt"">
                 <head>
                  <meta charset=""UTF-8"">
                  <meta content=""width=device-width, initial-scale=1"" name=""viewport"">
                  <meta name=""x-apple-disable-message-reformatting"">
                  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                  <meta content=""telephone=no"" name=""format-detection"">
                  <title>CIPA - FATEC JAHU</title><!--[if (mso 16)]>
                    <style type=""text/css"">
                    a {{text-decoration: none;}}
                    </style>
                    <![endif]--><!--[if gte mso 9]><style>sup {{ font-size: 100% !important; }}</style><![endif]--><!--[if gte mso 9]>
                <noscript>
                         <xml>
                           <o:OfficeDocumentSettings>
                           <o:AllowPNG></o:AllowPNG>
                           <o:PixelsPerInch>96</o:PixelsPerInch>
                           </o:OfficeDocumentSettings>
                         </xml>
                      </noscript>
                <![endif]--><!--[if !mso]><!-- -->
                  <link href=""https://fonts.googleapis.com/css2?family=Montserrat:wght@400;700&display=swap"" rel=""stylesheet""><!--<![endif]--><!--[if mso]><xml>
                    <w:WordDocument xmlns:w=""urn:schemas-microsoft-com:office:word"">
                      <w:DontUseAdvancedTypographyReadingMail/>
                    </w:WordDocument>
                    </xml><![endif]-->
                  <style type=""text/css"">
                .rollover:hover .rollover-first {{
                  max-height:0px!important;
                  display:none!important;
                }}
                .rollover:hover .rollover-second {{
                  max-height:none!important;
                  display:block!important;
                }}
                .rollover span {{
                  font-size:0px;
                }}
                u + .body img ~ div div {{
                  display:none;
                }}
                #outlook a {{
                  padding:0;
                }}
                span.MsoHyperlink,
                span.MsoHyperlinkFollowed {{
                  color:inherit;
                  mso-style-priority:99;
                }}
                a.es-button {{
                  mso-style-priority:100!important;
                  text-decoration:none!important;
                }}
                a[x-apple-data-detectors],
                #MessageViewBody a {{
                  color:inherit!important;
                  text-decoration:none!important;
                  font-size:inherit!important;
                  font-family:inherit!important;
                  font-weight:inherit!important;
                  line-height:inherit!important;
                }}
                .es-desk-hidden {{
                  display:none;
                  float:left;
                  overflow:hidden;
                  width:0;
                  max-height:0;
                  line-height:0;
                  mso-hide:all;
                }}
                .es-header-body a:hover {{
                  color:#ffffff!important;
                }}
                .es-content-body a:hover {{
                  color:#081D36!important;
                }}
                .es-footer-body a:hover {{
                  color:#081D36!important;
                }}
                .es-infoblock a:hover {{
                  color:#cccccc!important;
                }}
                .es-button-border:hover > a.es-button {{
                  color:#ffffff!important;
                }}
                @media only screen and (max-width:600px) {{.es-m-p20r {{ padding-right:20px!important }} .es-m-p20l {{ padding-left:20px!important }} .es-p-default {{ }} *[class=""gmail-fix""] {{ display:none!important }} p, a {{ line-height:150%!important }} h1, h1 a {{ line-height:120%!important }} h2, h2 a {{ line-height:120%!important }} h3, h3 a {{ line-height:120%!important }} h4, h4 a {{ line-height:120%!important }} h5, h5 a {{ line-height:120%!important }} h6, h6 a {{ line-height:120%!important }} .es-header-body p {{ }} .es-content-body p {{ }} .es-footer-body p {{ }} .es-infoblock p {{ }} h1 {{ font-size:30px!important; text-align:center }} h2 {{ font-size:24px!important; text-align:center }} h3 {{ font-size:20px!important; text-align:center }} h4 {{ font-size:24px!important; text-align:left }} h5 {{ font-size:20px!important; text-align:left }} h6 {{ font-size:16px!important; text-align:left }} .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a {{ font-size:30px!important }} .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a {{ font-size:24px!important }} .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a {{ font-size:20px!important }} .es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a {{ font-size:24px!important }} .es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a {{ font-size:20px!important }} .es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a {{ font-size:16px!important }} .es-menu td a {{ font-size:12px!important }} .es-header-body p, .es-header-body a {{ font-size:14px!important }} .es-content-body p, .es-content-body a {{ font-size:14px!important }} .es-footer-body p, .es-footer-body a {{ font-size:12px!important }} .es-infoblock p, .es-infoblock a {{ font-size:12px!important }} .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3, .es-m-txt-c h4, .es-m-txt-c h5, .es-m-txt-c h6 {{ text-align:center!important }} .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3, .es-m-txt-r h4, .es-m-txt-r h5, .es-m-txt-r h6 {{ text-align:right!important }} .es-m-txt-j, .es-m-txt-j h1, .es-m-txt-j h2, .es-m-txt-j h3, .es-m-txt-j h4, .es-m-txt-j h5, .es-m-txt-j h6 {{ text-align:justify!important }} .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3, .es-m-txt-l h4, .es-m-txt-l h5, .es-m-txt-l h6 {{ text-align:left!important }} .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img {{ display:inline!important }} .es-m-txt-r .rollover:hover .rollover-second, .es-m-txt-c .rollover:hover .rollover-second, .es-m-txt-l .rollover:hover .rollover-second {{ display:inline!important }} .es-m-txt-r .rollover span, .es-m-txt-c .rollover span, .es-m-txt-l .rollover span {{ line-height:0!important; font-size:0!important; display:block }} .es-spacer {{ display:inline-table }} a.es-button, button.es-button {{ font-size:18px!important; padding:10px 20px 10px 20px!important; line-height:120%!important }} a.es-button, button.es-button, .es-button-border {{ display:inline-block!important }} .es-m-fw, .es-m-fw.es-fw, .es-m-fw .es-button {{ display:block!important }} .es-m-il, .es-m-il .es-button, .es-social, .es-social td, .es-menu {{ display:inline-block!important }} .es-adaptive table, .es-left, .es-right {{ width:100%!important }} .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header {{ width:100%!important; max-width:600px!important }} .adapt-img {{ width:100%!important; height:auto!important }} .es-mobile-hidden, .es-hidden {{ display:none!important }} .es-desk-hidden {{ width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important }} tr.es-desk-hidden {{ display:table-row!important }} table.es-desk-hidden {{ display:table!important }} td.es-desk-menu-hidden {{ display:table-cell!important }} .es-menu td {{ width:1%!important }} table.es-table-not-adapt, .esd-block-html table {{ width:auto!important }} .h-auto {{ height:auto!important }} }}
                @media screen and (max-width:384px) {{.mail-message-content {{ width:414px!important }} }}
                </style>
                 </head>
                 <body class=""body"" style=""width:100%;height:100%;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0"">
                  <div dir=""ltr"" class=""es-wrapper-color"" lang=""pt"" style=""background-color:#091520""><!--[if gte mso 9]>
			                <v:background xmlns:v=""urn:schemas-microsoft-com:vml"" fill=""t"">
				                <v:fill type=""tile"" color=""#091520""></v:fill>
			                </v:background>
		                <![endif]-->
                   <table width=""100%"" cellspacing=""0"" cellpadding=""0"" class=""es-wrapper"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#091520"">
                     <tr>
                      <td valign=""top"" style=""padding:0;Margin:0"">
                       <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-header"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top"">
                       </table>
                       <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-footer"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top"">
                       </table>
                       <table cellspacing=""0"" cellpadding=""0"" align=""center"" class=""es-content"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important"">
                       </table>
                       <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-content"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important"">
                         <tr>
                          <td align=""center"" style=""padding:0;Margin:0"">
                           <table bgcolor=""#ffffff"" align=""center"" cellpadding=""0"" cellspacing=""0"" class=""es-content-body"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;border-radius:50px 50px 0 0;width:600px"" role=""none"">
                             <tr>
                              <td align=""left"" style=""padding:0;Margin:0"">
                               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                 <tr>
                                  <td align=""center"" valign=""top"" style=""padding:0;Margin:0;width:600px"">
                                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" bgcolor=""#b2222d"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;border-radius:49px 49px 0 0;background-color:#b2222d"" role=""presentation"">
                                     <tr>
                                      <td align=""center"" style=""padding:0;Margin:0;font-size:0px""><a target=""_blank"" href=""https://viewstripo.email"" style=""mso-line-height-rule:exactly;text-decoration:underline;color:#081D36;font-size:18px""><img src=""https://fwfnboc.stripocdn.email/content/guids/CABINET_8f176bf7db58f7255bd7cabe4269003b80af9e849e4bd59c111eb5a0c23c2c7e/images/image.png"" alt=""CIPA - FATEC JAHU"" width=""350"" title=""CIPA - FATEC JAHU"" class=""adapt-img"" style=""display:block;font-size:18px;border:0;outline:none;text-decoration:none;border-radius:49px 49px 0px 0px""></a></td>
                                     </tr>
                                   </table></td>
                                 </tr>
                               </table></td>
                             </tr>
                             <tr>
                              <td align=""left"" style=""padding:0;Margin:0"">
                               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                 <tr>
                                  <td align=""center"" valign=""top"" style=""padding:0;Margin:0;width:600px"">
                                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                     <tr>
                                      <td align=""center"" class=""es-m-p20r es-m-p20l"" style=""padding:0;Margin:0;padding-top:20px""><h1 style=""Margin:0;font-family:Montserrat, helvetica, arial, sans-serif;mso-line-height-rule:exactly;letter-spacing:0;font-size:40px;font-style:120%;font-weight:normal;line-height:48px;color:#081D36""><strong>Fale Conosco!</strong></h1></td>
                                     </tr>
                                     <tr>
                                      <td align=""center"" class=""es-m-p20r es-m-p20l"" style=""Margin:0;padding-top:20px;padding-right:90px;padding-bottom:10px;padding-left:90px;font-family:arial, 'helvetica neue', helvetica, sans-serif""><p style=""Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:27px;letter-spacing:0;color:#081D36;font-size:18px""><strong>Remetente:</strong> {model.Name}<br><strong>Email:</strong> {model.Email}<br><strong>Assunto:</strong> {model.Subject}<br><strong>Mensagem:</strong> {model.Text}<br> </p></td>
                                     </tr>
                                   </table></td>
                                 </tr>
                               </table></td>
                             </tr>
                             <tr>
                              <td align=""left"" style=""Margin:0;padding-top:30px;padding-right:20px;padding-bottom:30px;padding-left:20px"">
                               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                 <tr>
                                  <td align=""center"" valign=""top"" style=""padding:0;Margin:0;width:560px"">
                                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                     <tr>
                                      <td align=""center"" style=""padding:0;Margin:0""><!--[if mso]><a href=""https://viewstripo.email"" target=""_blank"" hidden>
	                <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" esdevVmlButton href=""https://viewstripo.email"" style=""height:51px; v-text-anchor:middle; width:187px"" arcsize=""0%"" stroke=""f""  fillcolor=""#b2222d"">
		                <w:anchorlock></w:anchorlock>
		                <center style='color:#ffffff; font-family:arial, ""helvetica neue"", helvetica, sans-serif; font-size:18px; font-weight:400; line-height:18px;  mso-text-raise:1px'>Responder</center>
	                </v:roundrect></a>
                <![endif]--><!--[if !mso]><!-- --><span class=""es-button-border msohide"" style=""border-style:solid;border-color:#2CB543;background:#fb5607;border-width:0px;display:inline-block;border-radius:0px;width:auto;mso-hide:all""><a  href=""mailto:{model.Email}"" target=""_blank"" class=""es-button es-button-1665060087841"" style=""mso-style-priority:100 !important;text-decoration:none !important;mso-line-height-rule:exactly;color:#ffffff;font-size:18px;padding:15px 40px;display:inline-block;background:#B2222D;border-radius:0px;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:21.6px;width:auto;text-align:center;letter-spacing:0;mso-padding-alt:0;mso-border-alt:10px solid #B2222D"">Responder</a></span><!--<![endif]--></td>
                                     </tr>
                                   </table></td>
                                 </tr>
                               </table></td>
                             </tr>
                           </table></td>
                         </tr>
                       </table>
                       <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-footer"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top"">
                         <tr>
                          <td align=""center"" style=""padding:0;Margin:0"">
                           <table bgcolor=""#ffffff"" align=""center"" cellpadding=""0"" cellspacing=""0"" class=""es-footer-body"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;border-radius:0 0 50px 50px;width:600px"" role=""none"">
                             <tr>
                              <td align=""left"" style=""padding:20px;Margin:0"">
                               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                 <tr>
                                  <td align=""center"" valign=""top"" style=""padding:0;Margin:0;width:560px"">
                                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                     <tr>
                                      <td align=""center"" style=""padding:20px;Margin:0;font-size:0"">
                                       <table border=""0"" width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                         <tr>
                                          <td style=""padding:0;Margin:0;border-bottom:3px solid #ffbe0b;background:unset;height:0px;width:100%;margin:0px""></td>
                                         </tr>
                                       </table></td>
                                     </tr>
                                     <tr>
                                      <td align=""center"" style=""padding:0;Margin:0;padding-top:15px;padding-bottom:15px""><p style=""Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#081D36;font-size:14px"">Comissão Interna de Prevenção de Acidentes - FATEC JAHU</p></td>
                                     </tr>
                                     <tr>
                                      <td align=""center"" style=""Margin:0;padding-right:20px;padding-left:20px;padding-top:15px;padding-bottom:15px;font-size:0"">
                                       <table border=""0"" width=""60%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                         <tr>
                                          <td style=""padding:0;Margin:0;border-bottom:1px solid #ffbe0b;background:unset;height:0px;width:100%;margin:0px""></td>
                                         </tr>
                                       </table></td>
                                     </tr>
                                   </table></td>
                                 </tr>
                               </table></td>
                             </tr>
                           </table></td>
                         </tr>
                       </table>
                       <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-content"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important"">
                       </table></td>
                     </tr>
                   </table>
                  </div>
                 </body>
                </html>";

                await _emailService.SendEmailAsync("ADD_EMAIL", model.Subject, body);
                return RedirectToAction("Contact", new { success = true });
            }
            return View(model);
        }


    }
}
