
<div align="center">
  <img src="https://github.com/Evelyn-Cass/fatec-jahu-cipa/blob/main/images/cipa-fatec-jahu.png" alt="CIPA Logo" width="250"/>
</div>

---

[Portuguese](https://github.com/Evelyn-Cass/fatec-jahu-cipa/tree/main?tab=readme-ov-file#cipa---fatec-jahu) | [English](https://github.com/Evelyn-Cass/fatec-jahu-cipa/tree/main/English)

# 📌 CIPA - FATEC JAHU

The **CIPA - FATEC Jahu** project was developed as part of the **Interdisciplinary Project** of the **Multiplatform Software Development** course at FATEC Jahu. The web application aims to **digitize, organize, and efficiently provide access** to meeting minutes and other documents from the **Internal Commission for Accident Prevention (CIPA)**.

This initiative seeks to replace traditional physical storage methods, offering faster, safer, and more sustainable access to information.

## 🎓 Interdisciplinary Project

The Interdisciplinary Project is an evaluative activity involving the integration of multiple disciplines. In the 3rd semester of the course, the following subjects participated:

- **Web Development II**
- **Agile Software Project Management**
- **Non-Relational Databases**

### Project objectives:

- **Knowledge Integration:** Bridge theory and practice through a real-world application.
- **Practical Application:** Enable students to solve real problems using technology.
- **Collaboration:** Encourage teamwork and the use of agile methodologies.
- **Innovation:** Develop functional, modern, and useful digital solutions.
- **Professional Preparation:** Provide hands-on experience aligned with market expectations.

## 📚 Documentation

Complete documentation, including technical details, features, and usage instructions, is available at:

🔗 [Access Documentation](https://github.com/Evelyn-Cass/fatec-jahu-cipa/tree/main/Documentation)

## 🌐 Application

The web application was built using modern technologies to ensure performance, usability, and security.

- **[Visit the application site](#)** *(link will be updated upon deployment)*
- **[Project Repository](https://github.com/Evelyn-Cass/cipa-fatec-jahu)**

## ⚙️ Prerequisites

To run the application locally, make sure you have:

- [.NET 6 SDK or later](https://dotnet.microsoft.com/download)
- [MongoDB Community Server](https://www.mongodb.com/try/download/community)
- [Visual Studio 2022 or later](https://visualstudio.microsoft.com/)
- Git (optional, for cloning the repository)

## 🔧 Setting Up the Environment

### 1. Clone the repository
```bash
git clone https://github.com/Evelyn-Cass/cipa-fatec-jahu.git
cd cipa-fatec-jahu
```

### 2. Set up the MongoDB connection string  
Edit the `appsettings.json` file:
```json
"MongoDB": {
  "ConnectionString": "mongodb://localhost:27017",
  "Database": "CipaFatecJahu"
}
```

### 3. Configure the email service
Also in `appsettings.json`, update the email settings:
```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderName": "CIPA - FATEC JAHU",
  "SenderEmail": "YOUR_EMAIL@gmail.com",
  "Username": "YOUR_EMAIL@gmail.com",
  "Password": "YOUR_APP_PASSWORD"
}
```

### 4. Update the manual email address  
In the `Controllers/ContactController.cs` file, update the following line:
```csharp
await _emailService.SendEmailAsync("YOUR_EMAIL@gmail.com", model.Subject, body);
```

### 5. Run the application  
Open the project in Visual Studio and press `F5` or run via terminal:
```bash
dotnet restore
dotnet run
```

## 🔐 Default Admin Credentials

The application includes a default **administrator user** for testing purposes:

- **Email:** adm@adm.com  
- **Password:** Administrador@1

> It is recommended to change these credentials in a production environment.

## 📬 Contact

For questions, suggestions, or feedback, feel free to reach out:

**Evelyn Cassinotte**  
[evelyn.cassinotte@fatec.sp.gov.br](mailto:evelyn.cassinotte@fatec.sp.gov.br)
