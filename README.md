
<div align="center">
  <img src="https://github.com/Evelyn-Cass/fatec-jahu-cipa/blob/main/images/cipa-fatec-jahu.png" alt="Logotipo da CIPA" width="250"/>
</div>

---

[Português](#cipa---fatec-jahu) | [English](#cipa---fatec-jahu-1)

# 📌 CIPA - FATEC JAHU

O projeto **CIPA - FATEC Jahu** foi desenvolvido como parte do **Projeto Interdisciplinar** do curso de **Desenvolvimento de Software Multiplataforma** da FATEC Jahu. A aplicação web tem como finalidade a **digitalização, organização e disponibilização eficiente** das atas de reuniões e demais documentos da **Comissão Interna de Prevenção de Acidentes (CIPA)**.

## 🎓 Projeto Interdisciplinar

O Projeto Interdisciplinar é uma atividade avaliativa que envolve a integração de múltiplas disciplinas. No 3º semestre do curso, participaram as disciplinas:

- **Desenvolvimento Web II**
- **Gestão Ágil de Projetos de Software**
- **Banco de Dados Não Relacional**

### Objetivos do projeto:

- **Integração de conhecimentos:** Relacionar teoria e prática por meio de uma aplicação real.
- **Aplicação prática:** Permitir aos alunos resolver problemas reais com tecnologia.
- **Colaboração:** Estimular o trabalho em equipe e o uso de metodologias ágeis.
- **Inovação:** Desenvolver soluções digitais funcionais, modernas e úteis.
- **Preparação profissional:** Proporcionar uma experiência prática próxima à realidade do mercado.

## 📚 Documentação

A documentação completa do projeto, contendo detalhes técnicos, funcionalidades e orientações de uso, está disponível em:

🔗 [Acessar Documentação](https://github.com/Evelyn-Cass/fatec-jahu-cipa/tree/main/Documentation)

## 🌐 Aplicação

A aplicação web foi construída utilizando tecnologias modernas para garantir desempenho, usabilidade e segurança.

- **[Acessar o site da aplicação](#)** *(link será atualizado quando publicado)*
- **[Repositório do Projeto](https://github.com/Evelyn-Cass/cipa-fatec-jahu)**

## ⚙️ Pré-requisitos

Para executar a aplicação localmente, você precisará ter instalado:

- [.NET 6 SDK ou superior](https://dotnet.microsoft.com/download)
- [MongoDB Community Server](https://www.mongodb.com/try/download/community)
- [Visual Studio 2022 ou superior](https://visualstudio.microsoft.com/)
- Git (opcional, para clonar o repositório)

## 🔧 Configuração do Ambiente

### 1. Clone o repositório
```bash
git clone https://github.com/Evelyn-Cass/cipa-fatec-jahu.git
cd cipa-fatec-jahu
```

### 2. Configure a string de conexão com o MongoDB  
Edite o arquivo `appsettings.json`:
```json
"MongoDB": {
  "ConnectionString": "mongodb://localhost:27017",
  "Database": "CipaFatecJahu"
}
```

### 3. Configure o serviço de e-mail
Ainda em `appsettings.json`, atualize os dados do remetente:
```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderName": "CIPA - FATEC JAHU",
  "SenderEmail": "SEU_EMAIL@gmail.com",
  "Username": "SEU_EMAIL@gmail.com",
  "Password": "SUA_SENHA_DE_APP"
}
```

### 4. Atualize o endereço de envio manual  
No arquivo `Controllers/ContactController.cs`, edite a linha:
```csharp
await _emailService.SendEmailAsync("SEU_EMAIL@gmail.com", model.Subject, body);
```

### 5. Execute a aplicação  
Abra o projeto no Visual Studio e pressione `F5` ou, via terminal:
```bash
dotnet restore
dotnet run
```

## 🔐 Credenciais de Acesso Inicial

A aplicação inclui um **usuário administrador padrão**, para fins de teste:

- **E-mail:** adm@adm.com  
- **Senha:** Administrador@1

> Recomenda-se alterar essas credenciais em ambiente de produção.

## 📬 Fale Conosco

Para dúvidas, sugestões ou contribuições, entre em contato:

**Evelyn Cassinotte**  
[evelyn.cassinotte@fatec.sp.gov.br](mailto:evelyn.cassinotte@fatec.sp.gov.br)
