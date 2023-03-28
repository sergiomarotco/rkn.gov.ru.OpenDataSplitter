namespace ParseXML
{

    /*
    /// <summary>
    /// Инофрмация об Операторе ПДн
    /// </summary>
    public class RKN_record
    {
        /// <summary>
        /// Номер Оператора в Реестре операторов (Регистрационный номер)
        /// </summary>
        public string pd_operator_num;
        /// <summary>
        /// Дата внесения оператора в Реестр операторов
        /// </summary>
        public string enter_date;
        /// <summary>
        /// Номер приказа РКН о внесении в Реестр операторов
        /// </summary>
        public string enter_order;
        /// <summary>
        /// Статус оператора
        /// </summary>
        public string status;
        /// <summary>
        /// Полное наименование оператора
        /// </summary>
        public string name_full;
        /// <summary>
        /// ИНН оператора
        /// </summary>
        public string inn;
        /// <summary>
        /// Адрес местонахождения
        /// </summary>
        public string address;
        /// <summary>
        /// Дата поступления уведомления
        /// </summary>
        public string income_date;
        /// <summary>
        /// Субъекты РФ, на территории которых происходит обработка персональных данных
        /// </summary>
        public string territory;
        /// <summary>
        /// Цель обработки персональных данных
        /// </summary>
        public string purpose_txt;
        /// <summary>
        /// Правовое основание обработки персональных данных
        /// </summary>
        public string basis;
        /// <summary>
        /// Список ИСПДн и их параметры
        /// </summary>
        public List<ISPDn> ISPDn_list;
        /// <summary>
        /// Дата начала обработки персональных данных
        /// </summary>
        public string startdate;
        /// <summary>
        /// Условие прекращения обработки персональных данных
        /// </summary>
        public string stop_condition;
        /// <summary>
        /// Срок прекращения обработки персональных данных
        /// </summary>
        public string stop_date;
        /// <summary>
        /// Основание внесения записи в реестр (номер приказа)
        /// </summary>
        public string enter_order_num;
        /// <summary>
        /// Дата основания внесения записи в реестр
        /// </summary>
        public string enter_order_date;
        /// <summary>
        /// Дата основания исключения из реестра
        /// </summary>
        public string end_order_date;
        /// <summary>
        /// Основание исключения из реестра (номер приказа)
        /// </summary>
        public string end_order_num;
        /// <summary>
        /// ФИО физического лица или наименование юридического лица, ответственных за обработку персональных данных
        /// </summary>
        public string resp_name;
        public RKN_record(
            string pd_operator_num,
            string enter_date,
            string enter_order,
            string status,
            string name_full,
            string inn,
            string address,
            string income_date,
            string territory,
            string purpose_txt,
            string basis,
            List<ISPDn> ISPDn_list,
            string startdate,
            string stop_condition,
            string stop_date,
            string enter_order_num,
            string enter_order_date,
            string end_order_date,
            string end_order_num,
            string resp_name
            )
        {
            this.pd_operator_num = pd_operator_num;
            this.enter_date = enter_date;
            this.enter_order = enter_order;
            this.status = status;
            this.name_full = name_full;
            this.inn = inn;
            this.address = address;
            this.income_date = income_date;
            this.territory = territory;
            this.purpose_txt = purpose_txt;
            this.basis = basis;
            this.ISPDn_list = ISPDn_list;
            this.startdate = startdate;
            this.stop_condition = stop_condition;
            this.stop_date = stop_date;
            this.enter_order_num = enter_order_num;
            this.enter_order_date = enter_order_date;
            this.end_order_date = end_order_date;
            this.end_order_num = end_order_num;
            this.resp_name = resp_name;
        }
    }*/

    /// <summary>
    /// ИСПДн оператора.
    /// </summary>
    public class ISPDn
    {
        /// <summary>
        /// Наименование ИС
        /// </summary>
        //public string name;
        /// <summary>
        /// Категории персональных данных
        /// </summary>
        public string pd_category;
        /// <summary>
        /// Категории субъектов, персональные данные которых обрабатываются
        /// </summary>
        public string category_sub_txt;
        /// <summary>
        /// Перечень действий с персональными данными
        /// </summary>
        public string actions_category;
        /// <summary>
        /// Обработка персональных данных
        /// </summary>
        public string pd_handle;
        /// <summary>
        /// Наличие трансграничной передачи
        /// </summary>
        public string transgran_transfer;
        /// <summary>
        /// Сведения о местонахождении базы данных
        /// </summary>
        public string db_country;
        /// <summary>
        /// Базовый класс создания ИСПДн
        /// </summary>
        public ISPDn(/*string name,*/ string pd_category, string category_sub_txt, string actions_category, string pd_handle, string transgran_transfer, string db_country)
        {
            //this.name = name;// нет этого параметра в открытых данных
            this.pd_category = pd_category;
            this.category_sub_txt = category_sub_txt;
            this.actions_category = actions_category;
            this.pd_handle = pd_handle;
            this.transgran_transfer = transgran_transfer;
            this.db_country = db_country;
        }
    }
}