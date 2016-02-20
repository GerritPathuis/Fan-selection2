﻿Imports System.Text
Imports System
Imports System.Configuration
Imports System.Math
Imports System.Collections.Generic
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Globalization
Imports System.Threading

Public Structure Tmodel
    Public Tname As String              'Name
    '0-Diameter,1-Toerental,2-Dichtheid,3-Zuigmond diameter,4-Persmond lengte,5-Breedte huis,6-Lengte spiraal,7-a,8-b,9-c,10-d,11-e,
    '12-Schoeplengte,13-Aantal schoepen,14-Breedte inwendig,15-Breedte uitwendig,16-Keeldiameter,17-Inw. dia. schoepen,18-Intrede hoek,19-Uittrede hoek......
    Public Tdata() As Double
    Public Teff() As Double             'Rendement [%]
    Public Tverm() As Double            'Vermogen[kW]
    Public TPstat() As Double           'Statische druk
    Public TPdyn() As Double            'Dynamische druk
    Public TPtot() As Double            'Totale druk
    Public TFlow() As Double            'Debiet[m3/s]
    Public werkp_opT() As Double        'rendement, P_totaal [Pa],P_statisch [Pa], as_vermogen [kW], debiet[m3/sec]
    Public Geljon() As Double           'A,B,C,E,F,G,q0_min,q0_max
    Public TFlow_scaled() As Double     'Debiet[m3/s]
    Public TPstat_scaled() As Double    'Statische druk
    Public TPtot_scaled() As Double     'Totale druk
    Public Teff_scaled() As Double      'Rendement [%]
    Public Tverm_scaled() As Double     'Vermogen[kW]
End Structure

Public Class Form1
    Public Tschets(31) As Tmodel            'was 31
    Public cp_air As Double = 1.005         'Specific heat air

    '----- "Oude benaming;Norm:;EN10027-1;Werkstof;[mm/m1/100°C];Poisson ;kg/m3;E [Gpa];Rm (20c);Rp0.2(0c);Rp0.2(20c);Rp(50c);Rp(100c);Rp(150c);Rp(200c);Rp(250c);Rp(300c);Rp(350c);Rp(400c);Equiv-ASTM;Opmerking",
    Public Shared steel() As String =
     {"16M03;EN10028-2 UNS;16M03;1.5415;1.29;0.28;7850;192;440-590;225;265;260;235;225;215;200;170;160;150;A204-GrA;--",
    "Aluminium D54S;nt.Reg.Nr: DIN1745-1;AA5083 AIMo45Mn-H116;3.3547;2.54;0.33;2700;70;305;121;150;140;135;121;67;0;0;0;0;--;--",
    "Chromanite Alloy;Robert Zapp UNS;Cr19Mn10N;1.382;1.73;0.28;7810;200;800-1050;400;500;500;445;400;360;330;310;300;295;-;--",
    "Corten - A / B;EN10155 UNS;S355J2G1W;1.8962/63;1.29;0.28;7850;192;490-630;240;355;340;255;240;226;206;166;0;0;--;Max 300c	",
    "Dillimax 690T;Dill.HuttWerke;DSE690V;1.8928;1.29;0.28;7850;192;790-940;717;690;790;740;717;698;697;687;659;638;A517-GrA;--",
    "Domex 690XPD(E);EN10149-2 UNS;S700MCD(E);1.8974;1.29;0.28;7850;192;810;675;740;765;690;675;660;640;620;580;540;--;--",
    "Duplex(Avesta-2205);EN 10088-1 UfllW;X2CrNiMoN22-5-3 saisna;1.4462;1.4;0.28;7800;200;640-950;335;460;385;360;335;315;300;0;0;0;A240-S31803;Max 300c",
    "Hastelloy-C22;DIN Nr: ASTM UNS;NiCr21Mo14W 2277 B575 N06022;2.4602;1.25;0.29;9000;205;786-800;310;370;354;338;310;283;260;248;244;241;--;--",
    "Inconel- 600;DIN Nicrofer7216 ASTM SO ;NiCr15Fe Alloy 600 B168 NiCr15Fe8 Npsepo;2.4816;1.44;0.29;8400;214;550;170;240;185;180;170;165;160;155;152;150;--;--",
    "Naxtra 70;Thyssen/DIN UNS;TSTE690V;1.8928;1.29;0.28;7850;192;790-940;635;690;700;660;635;605;585;570;550;530;A517-GrA;--",
    "P265GH;EN 10028-2 UNS;P265GH ;1.0425;1.29;0.28;7850;192;410-530;205;255;234;215;205;195;175;155;140;130;A516-Gr60;--",
    "S235JRG2;EN 10025 UNS;S235JRG2 ;1.0038;1.29;0.28;7850;192;340-470;180;195;200;190;180;170;150;130;120;110;A283-GrC;--",
    "S355J2G3;EN10025 UNS;S355J2G3;1.057;1.29;0.28;7850;192;490-630;284;315;340;304;284;255;226;206;0;0;A299;Max 300c	",
    "SS 304;EN10088-2;X5CrNI18-10 S30400;1.4301;1.76;0.28;7900;200;520-750;142;210;165;157;142;127;118;110;104;98;A240-304;--",
    "SS 304L;EN10088-2;X2CrNi19-11 S30403;1.4306;1.76;0.28;7900;200;520-670;132;200;155;147;132;118;108;100;94;89;A240-304L;--",
    "SS 316;EN10088-2;X5CrNiMo17-12-2 S31600;1.4401;1.76;0.28;8000;200;520-680;162;220;180;177;162;147;137;127;120;115;A240-316;--",
    "SS 316TI;EN10088-2;X6CrNiMoTi17-12-2 S31635;1.4571;1.76;0.28;8000;200;520-690;177;220;191;185;177;167;157;145;140;135;A240-316Ti;--",
    "SS 321;EN10088-2;X6CrNiTi18-10 S32100;1.4541;1.76;0.28;7900;200;500-720;167;200;184;176;167;157;147;136;130;125;A240-321;--",
    "SS 410 ;EN 10088-1 U1S;X12Cr13 (Gegloeid) 541000;1.4006;1.15;0.28;7700;216;450-650;230;250;240;235;230;225;225;220;210;195;A240-410;--",
    "SS316L;EN10088-2;X2CrNiMo17-12-2 S31603;1.4404;1.76;0.28;8000;200;520-680;152;220;170;166;152;137;127;118;113;108;A240-316L;--",
    "SuperDuplex;--;X2CrNiMoN22-5-3 saisna;1.4501;1.4;0.28;7800;200;730-930;445;550;510;480;445;405;400;395;0;0;--;--",
    "Titanium-ür 2;ASTM UNS niN;B265/348-Gr2 R50400 785(1;3.7035;0.88;0.32;4500;107;345;177;281;245;226;177;131;99;80;0;0;--;Max 280c i.v.m verbrossing",
    "Weldox700E;EN10137-2 UNS;S690QL;1.8928;1.29;0.28;7850;192;780-930;590;700;643;600;590;580;570;560;550;540;--;--",
    "WSTE/TSTE355;EN 10028-3 UNS;P355NH/NL1;1.0565/66;1.29;0.28;7850;192;470-630;284;315;340;304;284;255;226;206;186;157;A516-Gr70;--"}

    'Motoren
    Public Shared emotor() As String = {"4.0; 3000", "5.5; 3000", "7.5; 3000", "11;  1500", "15; 3000", "22; 3000",
                                       "30;   3000", "37;  3000", "45;  3000", "55;  3000", "75; 3000", "90; 3000",
                                       "110;  3000", "132; 3000", "160; 3000", "200; 3000", "250; 3000", "315; 3000",
                                       "355;  3000", "400; 3000", "450; 3000", "500; 3000", "560; 3000", "630; 3000"}

    Public Shared EXD_VSD_torque() As String = {"Hz; rpm; Koppel_%",
                                       "0 ; 0; 56",
                                       "5 ; 149; 75",
                                       "10; 297; 81",
                                       "15; 446; 85.5",
                                       "20; 595; 90",
                                       "25; 744; 92",
                                       "30; 892; 94",
                                       "35; 1041; 96",
                                       "40; 1190; 98",
                                       "45; 1338; 100",
                                       "50; 1487; 90",
                                       "55; 1636, 83",
                                       "60; 1784; 76",
                                       "65; 1933; 69",
                                       "70; 2082; 63",
                                       "75; 2231; 57.5",
                                       "80; 2379; 53.5",
                                       "85; 2528; 49",
                                       "90; 2677; 46",
                                       "95; 2825; 43.5",
                                       "100; 2974; 42"}

    Dim flenzen() As Double = {71, 80, 90, 100, 112, 125, 140, 160, 180, 200, 224, 250, 280, 315, 355, 400, 450, 500, 560, 630, 710, 800, 900, 1000, 1120, 1250, 1400, 1600, 1800, 2000}
    Dim R20() As Double

    'T-model, Alle gegevens bij het hoogste rendement
    Public T_eff As Double              'Efficiency max [-]
    Public T_Ptot_Pa As Double          'Pressure totaal [Pa]
    Public T_PStat_Pa As Double         'Pressure Statisch [Pa]
    Public T_Power_opt As Double        'Power optimal point [kW]
    Public T_Toerental_sec As Double    'Toerental [/sec]
    Public T_Toerental_rpm As Double    'Toerental [rpm]
    Public T_Debiet_sec As Double       'Debiet [m3/sec]
    Public T_Debiet_hr As Double        'Debiet [m3/hr] 
    Public T_Debiet_kg_sec As Double    'Debiet [kg/sec] 
    Public T_sg_gewicht As Double       'Soortelijk gewicht [kg/m3]
    Public T_diaw_m As Double           'Diameter waaier [m]
    Public T_no_schoep As Double        'Aantal schoepen [-]
    Public T_hoek_in As Double          'Schoep intrede hoek
    Public T_hoek_uit As Double         'Schoep uittrede hoek
    Public T_omtrek_s As Double         'Waaier omtreksnelhied [m/s]
    Public T_as_kw As Double            'Opgenomen vermogen [kw]
    Public T_visco_kin As Double        'Viscositeit lucht kinamatic [m2/s]
    Public T_reynolds As Double         'Reynolds waaier [-]
    Public T_air_temp As Double         'Lucht temperatuur inlet [celcius]
    Public T_spec_labour As Double      'Specifieke arbeid [J/kg]
    Public T_Totaaldruckzahl As Double  'Kental [-]
    Public T_Volumezahl As Double       'Kental [-]
    Public T_laufzahl As Double         'Laufzahl kengetal [-]
    Public T_Drehzahl As Double         'Drehzahl kengetal [-]
    Public T_durchmesser_zahl As Double 'Durchesserzahl kengetal [-]

    'Gewenste gegevens, Alle gegevens bij het hoogste rendement
    Public G_eff As Double              'Efficiency max [-]
    Public G_Ptot_Pa As Double          'Pressure totaal [Pa]
    Public G_Ptot_mBar As Double        'Pressure totaal [mBar]
    Public G_Toerental_rpm As Double    'Toerental [rpm]
    Public G_Debiet_z_act_sec As Double 'Debiet zuig [Am3/sec] (A= actual)
    Public G_Debiet_z_act_hr As Double  'Debiet zuig [Am3/hr] (A= actual)
    Public G_Debiet_z_N_hr As Double    'Debiet zuig [Nm3/hr] (normal density, 1013.25 mbar, sea level, 0 celsius)
    Public Gas_mol_weight As Double     'Gas mol gewicht [kg/mol]

    Public G_Debiet_p As Double         'Debiet pers [m3/sec]
    Public G_Debiet_kg_s As Double      'Debiet [kg/sec]
    Public G_Debiet_kg_hr As Double     'Debiet [kg/hr]
    Public G_density_act_zuig As Double 'Soortelijk gewicht [kg/Am3] (actual density)
    Public G_density_act_pers As Double 'Soortelijk gewicht [kg/Am3] (actual density)
    Public G_density_act_average As Double 'Soortelijk gewicht [kg/Am3] (actual gemiddeld)
    Public G_density_N_zuig As Double   'Soortelijk gewicht [kg/Nm3] (normal density, 1013.25 mbar, sea level, 0 celsius)
    Public G_density_N_pers As Double   'Soortelijk gewicht [kg/Nm3] (normal density, 1013.25 mbar, sea level, 0 celsius)

    Public G_Totaaldruckzahl As Double  'Kental
    Public G_omtrek_s As Double         'Omvangssnelheid waaier
    Public G_diaw_m As Double           'Diameter waaier [m]
    Public G_as_kw As Double            'Opgenomen vermogen
    Public G_visco_kin As Double        'Viscositeit lucht [m2/sec]
    Public G_reynolds As Double         'Reynolds waaier [-]
    Public G_air_temp As Double         'Lucht temperatuur in [celcius]
    Public G_temp_uit_c As Double       'Lucht temperatuur uit [c]

    'Renard gegevens
    Public Renard_diaw_m_R20 As Double       'Diameter waaier [m] in de R20 reeks
    Public Renard_Toerental_rpm As Double    'Toerental [rpm]
    Public Renard_omtrek_s As Double         'Waaier omtreksnelhied [m/s]
    Public Renard_as_kw As Double            'Opgenomen vermogen
    Public Renard_reynolds As Double         'Reynolds waaier [-]
    Public Renard_eff As Double              'Efficiency max [-]
    Public Renard_Debiet_z_sec As Double     'Debiet zuig [m3/sec]
    Public Renard_temp_uit_c As Double       'Lucht temperatuur uit [c]

    'Waaier direct gekoppelde aan de motor 
    Public Direct_diaw As Double             'Diameter waaier [m] berekend
    Public Direct_diaw_m_R20 As Double       'Diameter waaier [m] in de R20 reeks
    Public Direct_Toerental_rpm As Double    'Toerental [rpm] gekozen door gebruiker
    Public Direct_omtrek_s As Double         'Waaier omtreksnelhied [m/s]
    Public Direct_as_kw As Double            'Opgenomen vermogen
    Public Direct_reynolds As Double         'Reynolds waaier [-]
    Public Direct_eff As Double              'Efficiency max [-]
    Public Direct_Debiet_z_sec As Double     'Debiet zuig [m3/sec]
    Public Direct_temp_uit_c As Double       'Lucht temperatuur uit [c].

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim hh As Integer
        Dim words() As String

        fill_array_T_schetsen()                     'Init T-schetsen info in de array plaatsen

        Find_hi_eff()                               'Determine work points

        Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
        Thread.CurrentThread.CurrentUICulture = New CultureInfo("en-US")

        ComboBox1.Items.Clear()                     'Note Combobox1 contains "startup" to prevent exceptions
        ComboBox2.Items.Clear()                     'Note Combobox1 contains "startup" to prevent exceptions
        ComboBox3.Items.Clear()                     'Note Combobox1 contains "startup" to prevent exceptions
        ComboBox4.Items.Clear()                     'Note Combobox1 contains "startup" to prevent exceptions
        ComboBox5.Items.Clear()                     'Note Combobox1 contains "startup" to prevent exceptions
        ComboBox6.Items.Clear()                     'Note Combobox1 contains "startup" to prevent exceptions
        ComboBox7.Items.Clear()                     'Note Combobox1 contains "startup" to prevent exceptions


        For hh = 0 To (UBound(Tschets) - 1)            'Fill combobox 1,2 +5 met Fan Types
            ComboBox1.Items.Add(Tschets(hh).Tname)
            ComboBox2.Items.Add(Tschets(hh).Tname)
            ComboBox5.Items.Add(Tschets(hh).Tname)
            ComboBox7.Items.Add(Tschets(hh).Tname)
        Next hh

        '-------Fill combobox3, Steel selection------------------
        For hh = 0 To (UBound(steel) - 1)            'Fill combobox 3 with steel data
            words = steel(hh).Split(";")
            ComboBox3.Items.Add(words(0))
        Next hh

        Label34.Text = ChrW(963) & " 0.2 @ T bedrijf [N/mm]"

        '-------Fill combobox4, Motor selection------------------
        For hh = 0 To (UBound(emotor) - 1)            'Fill combobox 4 electric motor data
            words = emotor(hh).Split(";")
            ComboBox4.Items.Add(words(0))
            ComboBox6.Items.Add(words(0))
        Next hh


        '----------------- prevent out of bounds------------------
        If ComboBox1.Items.Count > 0 Then
            ComboBox1.SelectedIndex = 6                 'Select T17B
        End If
        If ComboBox2.Items.Count > 0 Then
            ComboBox2.SelectedIndex = 6                 'Select T17B
        End If
        If ComboBox5.Items.Count > 0 Then
            ComboBox5.SelectedIndex = 6                 'Select T17B
        End If
        If ComboBox7.Items.Count > 0 Then
            ComboBox7.SelectedIndex = 6                 'Select T17B
        End If
        If ComboBox4.Items.Count > 0 Then
            ComboBox4.SelectedIndex = 1
        End If
        If ComboBox6.Items.Count > 0 Then
            ComboBox6.SelectedIndex = 1
        End If


    End Sub

    Private Sub Selectie_1()

        Dim nrq As Integer
        Dim Rel_humidity As Double
        Dim P_systeem_Pa As Double          'Ring syteem onder druk Pressure abs in [Pa]
        Dim P_ambient_Pa As Double          'Pressure abs in [Pa]
        Dim P_zuig_Pa As Double             'Pressure abs in [Pa]
        Dim P_pers_Pa As Double             'Pressure abs in [Pa]
        Dim visco_temp As Double
        Dim site_altitude As Double
        Dim Ttype As Int16                  'Waaier type
        Dim diam1, diam2, nn1, nn2, roo1, roo2 As Double
        Dim WP2_stat1, WP2_total1 As Double   'Work_point_2 inlaat (gegevens uit de T-schetsen)
        Dim WP2_stat2, WP2_total2 As Double   'Work_point_2 uitlaat (berekend via de schaal regels)

        '0-Diameter,1-Toerental,2-Dichtheid,3-Zuigmond diameter,4-Persmond lengte,5-Breedte huis,6-Lengte spiraal,7 breedte pers,8 lengte pers,9-c,10-d,11-e,
        '12-Schoeplengte,13-Aantal schoepen,14-Breedte inwendig,15-Breedte uitwendig,16-Keeldiameter,17-Inw. dia. schoepen,18-Intrede hoek,19-Uittrede hoek

        nrq = ComboBox7.SelectedIndex   'Prevent out of bounds error
        If nrq >= 0 And nrq <= 30 Then
            Try
                '------- get data from database--------------------------
                TextBox1.Text = Tschets(nrq).Tdata(0)           'Diameter waaier. 

                T_sg_gewicht = Tschets(nrq).Tdata(2)            'soortelijk gewicht lucht .
                TextBox14.Text = Round(T_sg_gewicht, 2)         'Lucht s.g.[kg/m3]
                TextBox7.Text = Tschets(nrq).Tdata(3)           'Zuigmond diameter.
                TextBox2.Text = Tschets(nrq).Tdata(4)           'Uitlaat hoogte inw. 
                TextBox3.Text = Tschets(nrq).Tdata(5)           'Uitlaat breedte inw.
                TextBox8.Text = Tschets(nrq).Tdata(6)           'Lengte spiraal

                TextBox61.Text = Tschets(nrq).Tdata(12)         'schoep lengte.
                TextBox9.Text = Tschets(nrq).Tdata(13)          'aantal schoepen.
                TextBox5.Text = Tschets(nrq).Tdata(14)          'Inw schoep breedte.
                TextBox4.Text = Tschets(nrq).Tdata(15)          'Uitw schoep breedte.
                TextBox6.Text = Tschets(nrq).Tdata(16)          'Keel diameter
                TextBox62.Text = Tschets(nrq).Tdata(17)         'inwendige schoep diameter 
                TextBox59.Text = Tschets(nrq).Tdata(18)         'schoep intrede hoek.
                TextBox60.Text = Tschets(nrq).Tdata(19)         'schoep uittrede hoek.
                TextBox53.Text = Tschets(nrq).werkp_opT(3)      'As vermogen

                T_hoek_in = Convert.ToDouble(TextBox59.Text)    'schoep intrede hoek
                T_hoek_uit = Convert.ToDouble(TextBox60.Text)   'schoep uittrede hoek
                T_eff = Tschets(nrq).werkp_opT(0) / 100         'Rendement[-]

                T_Ptot_Pa = Tschets(nrq).werkp_opT(1)           'Pressure totaal [Pa]
                T_PStat_Pa = Tschets(nrq).werkp_opT(2)          'Pressure statisch [Pa]
                T_Power_opt = Tschets(nrq).werkp_opT(3)         'AS vermogen [kW]
                T_Toerental_rpm = Tschets(nrq).Tdata(1)         'Toerental [rpm]
                T_Toerental_sec = T_Toerental_rpm / 60.0                         'Toerental [/sec]

                '------------ site altitude-----------------
                site_altitude = NumericUpDown7.Value                             'Site hoogte boven zee niveau

                '----------- temperaturen----------------
                T_air_temp = 20                                                 'T-schetsen proef temperatuur

                '---------- debiet----------------------
                T_Debiet_sec = Tschets(nrq).werkp_opT(4)    'Debiet [Am3/sec]
                T_Debiet_hr = Round(T_Debiet_sec * 3600, 0)                     'Debiet [Am3/hr]
                T_Debiet_kg_sec = T_Debiet_sec * T_sg_gewicht                   'Debiet [kg/s]


                '-------------- waaier-------------------------------
                T_diaw_m = Convert.ToDouble(TextBox1.Text) / 1000               'Diameter waaier [m]
                T_as_kw = Convert.ToDouble(TextBox53.Text)                      'as_vermogen [kW]
                T_omtrek_s = T_diaw_m * PI * T_Toerental_sec                    'Omtrek snelheid 

                '---------- Specifieke arbeid, pagina 25 -----------------   
                T_spec_labour = T_Ptot_Pa / T_sg_gewicht                        'Spec arbeid [J/kg]

                '------------- visco----------------
                T_visco_kin = kin_visco_air(T_air_temp)                         'Kin viscositeit [m2/s] T_schets
                visco_temp = Round(T_visco_kin * 10 ^ 6, 2)
                TextBox69.Text = visco_temp.ToString        'Visco T_schets

                '-------------------------------------------------------------------------------------------------
                '---------- specifiek toerental kengetal [-] formule 2.2 pagina 40 -------------------------------
                T_Drehzahl = T_Toerental_sec * Sqrt(T_Debiet_sec / Pow(T_spec_labour / 9.81, 0.75))


                '---------- specifiek laufzahl kengetal [-] formule 2.1 pagina 40 --------------------------------
                T_laufzahl = T_Drehzahl / 157.8


                '---------- diameter toeretal kengetal [-] formule 2.5 pagina 41 ---------------------------------
                T_durchmesser_zahl = T_diaw_m * Pow(2 * T_spec_labour / T_Debiet_sec ^ 2, 0.25) * Sqrt(PI) / 2


                '--------------- Totaldruckzahl (Zie hoofdstuk 4.2,  pagina 130 )---------------------------------
                T_Totaaldruckzahl = 2 * T_Ptot_Pa / (T_sg_gewicht * T_omtrek_s ^ 2)

                '----------- Volume zahl----------------------------------------------------------------------------
                T_Volumezahl = 4 * T_Debiet_sec / (Pow(PI, 2) * Pow(T_diaw_m, 3) * T_Toerental_sec)

                '------------ Reynolds T-schets--------------------------------------------------------------------
                T_reynolds = Round(T_omtrek_s * T_diaw_m / T_visco_kin, 0)
                TextBox68.Text = Round((T_reynolds * 10 ^ -6), 2).ToString

                '-----------------Present T-model info----------------------------------------------
                TextBox10.Text = T_eff.ToString                             'Rendement
                TextBox11.Text = Round(T_Ptot_Pa, 0).ToString
                TextBox55.Text = Round(T_PStat_Pa, 0).ToString
                TextBox31.Text = T_Toerental_rpm.ToString                   '[rpm]
                TextBox13.Text = T_Debiet_sec.ToString
                TextBox56.Text = T_Debiet_hr.ToString
                TextBox30.Text = Round(T_omtrek_s, 1).ToString
                TextBox71.Text = T_air_temp.ToString
                TextBox17.Text = Round(T_Totaaldruckzahl, 3).ToString      'Totaldruckzahl
                TextBox18.Text = Round(T_Volumezahl, 3).ToString           'Volume zahl
                TextBox85.Text = Round(T_spec_labour, 1).ToString          'Specifieke arbeid [-]
                TextBox86.Text = Round(T_laufzahl, 3).ToString             'Laufzahl [-]
                TextBox87.Text = Round(T_Debiet_kg_sec, 2).ToString        'Debiet [kg/hr]
                TextBox88.Text = Round(T_durchmesser_zahl, 2).ToString        'Diameter kengetal [-]
                TextBox123.Text = Round(T_Drehzahl * 60, 0).ToString       'Spez Drehzahl [rpm]
                TextBox124.Text = Round(T_spec_labour, 0).ToString         'Spez. Arbeid [j/Kg]


                '--------------------------- gewenste gegevens------------------------------------------
                G_Ptot_mBar = NumericUpDown2.Value          'Gewenst Pressure totaal [mbar]
                G_Ptot_Pa = G_Ptot_mBar * 100               'Gewenst Pressure totaal [Pa]
                G_air_temp = NumericUpDown4.Value           'Gewenste arbeids temperatuur in [c]

                '------------ Gas mol Weight vochtigheid ---------------
                Gas_mol_weight = NumericUpDown8.Value / 1000        'Mol gewicht [kg/mol]

                '------------ Relatieve vochtigheid ---------------
                Rel_humidity = Convert.ToDouble(NumericUpDown5.Value)

                '---------------- Debiet in kgs------------------
                G_Debiet_kg_hr = NumericUpDown3.Value
                G_Debiet_kg_s = G_Debiet_kg_hr / 3600       'Gewenst Debiet [kg/sec]

                '---------------- Gesloten systeem onder druk------------------
                P_systeem_Pa = NumericUpDown1.Value * 100

                '--------------- site hoogte---------------
                'Molgewicht lucht is (Gas_mol_weight) 0,0288 kg/mol
                'R= algemene gasconstante = 8,3144621
                P_ambient_Pa = Round(1013.15 * Pow(E, -Gas_mol_weight * 9.81 * site_altitude / (8.3144621 * (G_air_temp + 273.15))), 0) * 100

                '----------- Zuigdruk ----------------
                P_zuig_Pa = P_ambient_Pa - (NumericUpDown6.Value * 100) 'Inlet resctrictions reduce the pressure inside the fan

                P_zuig_Pa += P_systeem_Pa
                TextBox91.Text = Round(P_zuig_Pa.ToString / 100, 0)
                P_pers_Pa = P_zuig_Pa + G_Ptot_Pa


                '---------------Density berekenen of invullen----------------
                If RadioButton3.Checked = True Then     'Density berekenen
                    NumericUpDown12.Enabled = False
                    G_density_act_zuig = calc_sg_air(P_zuig_Pa, G_air_temp, Rel_humidity, Gas_mol_weight)       'Actual conditions zuig
                    G_density_N_zuig = calc_sg_air(101325, 0, Rel_humidity, Gas_mol_weight)                     'Normal conditions zuig
                    NumericUpDown12.Text = Round(G_density_act_zuig, 3).ToString                                'Density zuig
                    NumericUpDown12.BackColor = Color.White
                    NumericUpDown5.Visible = True          'Relative humidity
                    NumericUpDown7.Visible = True          'Site hoogte
                    Label95.Visible = True                 'Site hoogte
                    GroupBox16.Visible = True              'Molair weight
                Else
                    NumericUpDown12.Enabled = True          'Density invullen
                    G_density_act_zuig = NumericUpDown12.Value
                    G_density_N_zuig = calc_sg_air(101325, 0, Rel_humidity, Gas_mol_weight)                     'Normal conditions zuig
                    NumericUpDown12.BackColor = Color.Yellow
                    NumericUpDown5.Visible = False          'Relative humidity  
                    NumericUpDown7.Visible = False          'Site hoogte
                    Label95.Visible = False                 'Site hoogte
                    GroupBox16.Visible = False              'Molair weight
                End If


                '---------------- Debiet in m3------------------
                G_Debiet_z_act_sec = G_Debiet_kg_s / G_density_act_zuig     'Gewenst Debiet [Am3/hr]
                G_Debiet_z_act_hr = G_Debiet_z_act_sec * 3600.0             'Gewenst Debiet [Am3/hr]
                G_Debiet_z_N_hr = G_Debiet_kg_s / G_density_N_zuig * 3600   'Gewenst Debiet [Nm3/hr]
                G_Debiet_p = G_Debiet_kg_s / G_density_act_pers             'Pers Debiet [Am3/hr]

                '----------- de gewenste waaier---------------
                G_omtrek_s = Pow(2 * G_Ptot_Pa / (G_density_act_zuig * T_Totaaldruckzahl), 0.5)
                G_diaw_m = Pow(4 * (G_Debiet_z_act_sec) / (PI * T_Volumezahl * G_omtrek_s), 0.5)
                G_Toerental_rpm = (G_omtrek_s / (PI * G_diaw_m)) * 60

                '---------- as vermogen gewenste waaier-----------
                G_as_kw = 0.001 * G_Debiet_z_act_sec * G_Ptot_Pa / T_eff    'Go to Kw

                '---------- temperaturen, lost power is tranferred to heat -----------
                G_temp_uit_c = G_air_temp + (G_as_kw / (cp_air * G_Debiet_kg_s))

                '----------------- Actual conditions at discharge ----------------------------
                G_density_act_pers = calc_density(G_density_act_zuig, P_zuig_Pa, P_pers_Pa, G_air_temp, G_temp_uit_c)

                '---------- presenteren-----------------------
                TextBox16.Text = Round(G_temp_uit_c, 0).ToString           'Temp uit
                TextBox23.Text = Round(P_pers_Pa / 100, 3).ToString        'Pers druk in mbar abs
                TextBox24.Text = Round(G_density_N_zuig, 3).ToString
                TextBox25.Text = Round(G_density_act_pers, 3).ToString
                TextBox26.Text = Round(G_omtrek_s, 2).ToString             'Omtrek snelheid
                TextBox28.Text = Round(G_Debiet_p * 3600, 0).ToString      'Pers debiet is kleiner dan zuig debiet door drukverhoging
                TextBox27.Text = Round(G_diaw_m, 3).ToString
                TextBox29.Text = Round(G_Toerental_rpm, 0).ToString
                TextBox58.Text = Round(G_as_kw, 1).ToString
                TextBox20.Text = Round(G_Debiet_z_N_hr, 0).ToString       'Debiet Nm3/hr  
                TextBox22.Text = Round(G_Debiet_z_act_hr, 0).ToString     'Debiet Am3/hr  

                '==================================================================================================
                '----------------------------- Renard R20 reeks voor de waaier ------------------------------------
                If CheckBox5.Checked Then
                    Renard_diaw_m_R20 = find_Renard_R20(G_diaw_m)              'Diameter waaier [m] in de R20 reeks
                Else
                    Renard_diaw_m_R20 = Round(G_diaw_m, 2)                     'De berekende diameter
                End If
                Renard_omtrek_s = G_omtrek_s                                   'Omtrek snelheid blijft gelijk
                Renard_Toerental_rpm = Renard_omtrek_s / (PI * Renard_diaw_m_R20) * 60.0 'Toerental [rpm]

                '--------- Kinmatic viscosity air[m2/s]-----------------------
                G_visco_kin = kin_visco_air(G_air_temp)                         'Kin viscositeit [m2/s]
                visco_temp = Round(G_visco_kin * 10 ^ 6, 2)

                '------------ Reynolds Renard waaier  -------------------------------------------------------------
                Renard_reynolds = Round(Renard_omtrek_s * Renard_diaw_m_R20 / G_visco_kin, 0)
                TextBox72.Text = Round((Renard_reynolds * 10 ^ -6), 2).ToString

                '------------ Rendement Renard Waaier (Ackeret) --------------
                If CheckBox4.Checked Then
                    Renard_eff = 1 - 0.5 * (1 - T_eff) * Pow((1 + (T_reynolds / Renard_reynolds)), 0.2)
                Else
                    Renard_eff = T_eff
                End If

                '---------- as vermogen gewenste waaier-----------
                Renard_as_kw = 0.001 * G_Debiet_z_act_sec * G_Ptot_Pa / Renard_eff   'Go to kW

                '---------- temperaturen, lost power is tranferred to heat -----------
                Renard_temp_uit_c = G_air_temp + (Renard_as_kw / (cp_air * G_Debiet_kg_s))
                TextBox57.Text = Round(Renard_temp_uit_c, 0).ToString       'Temp uit

                '---------- Debiet -----------------------
                Renard_Debiet_z_sec = Renard_diaw_m_R20 ^ 2 * PI * Renard_omtrek_s * T_Volumezahl / 4

                '---------- presenteren-----------------------
                TextBox73.Text = Round(Renard_omtrek_s, 1)                  'Omtrek snelheid [m/s]
                TextBox70.Text = visco_temp.ToString                        'Visco T_schets
                TextBox74.Text = Round(Renard_eff, 3).ToString              'Efficiency
                TextBox65.Text = Renard_diaw_m_R20.ToString                 'diameter
                TextBox66.Text = Round(Renard_Toerental_rpm, 0).ToString    'Speed [rpm]
                TextBox67.Text = Round(Renard_as_kw, 0).ToString            'As vermogen in [kW]
                TextBox84.Text = Round(Renard_Debiet_z_sec * 3600.0, 0)     'Debiet m3/hr


                '========================= 2de Bedrijfspunt===================================================================
                Direct_Toerental_rpm = NumericUpDown13.Value                                'Kies het waaier toerental
                Direct_diaw = NumericUpDown33.Value / 1000                                  'Diameter [m]

                Direct_omtrek_s = PI * Direct_Toerental_rpm / 60 * Direct_diaw
                TextBox76.Text = Round(Direct_omtrek_s, 1)                                  'Omtrek snelheid [m/s]

                'If CheckBox5.Checked Then
                '    Direct_diaw_m_R20 = find_Renard_R20(Direct_diaw)                        'Diameter volgens R20 reeks
                'Else
                '    Direct_diaw_m_R20 = Round(Direct_diaw, 3)
                'End If
                Direct_diaw_m_R20 = Round(Direct_diaw, 3)

                '------------ Reynolds 2e bedrijfspunt -----------------------
                Direct_reynolds = Round(Direct_omtrek_s * Direct_diaw_m_R20 / G_visco_kin, 0)

                '------------ Rendement Waaier (Ackeret) --------------
                If CheckBox4.Checked Then
                    Direct_eff = 1 - 0.5 * (1 - T_eff) * Pow((1 + (T_reynolds / Direct_reynolds)), 0.2)
                Else
                    Direct_eff = T_eff
                End If

                '---------- Debiet 2e bedrijfspunt ----------------------------------------------
                Direct_Debiet_z_sec = Direct_diaw_m_R20 ^ 2 * PI * Direct_omtrek_s * T_Volumezahl / 4                                               'Deze formule werkt ook
                'Direct_Debiet_z_sec = Round(Scale_rule_cap(T_Debiet_sec, T_diaw_m, Direct_diaw_m_R20, T_Toerental_rpm, Direct_Toerental_rpm), 2)   '[m3/s]


                '---------- Pressure 2e bedrijfspunt -----------------------
                Ttype = ComboBox1.SelectedIndex
                diam1 = Tschets(Ttype).Tdata(0)             'waaier diameter [mm]
                diam2 = NumericUpDown33.Value
                nn1 = Tschets(Ttype).Tdata(1)               'waaier [rpm]
                nn2 = NumericUpDown13.Value
                roo1 = Tschets(Ttype).Tdata(2)              'density [kg/m3]
                roo2 = NumericUpDown12.Value
                WP2_total1 = Tschets(Ttype).werkp_opT(1)     'P_total [Pa]
                WP2_stat1 = Tschets(Ttype).werkp_opT(2)      'P_stat [Pa]

                WP2_total2 = Round(Scale_rule_Pressure(WP2_total1, diam1, diam2, nn1, nn2, roo1, roo2), 0)
                WP2_stat2 = Round(Scale_rule_Pressure(WP2_stat1, diam1, diam2, nn1, nn2, roo1, roo2), 0)

                TextBox81.Text = Round(WP2_total2 / 100, 3).ToString                    'dP_Total  [mbar]
                TextBox150.Text = Round(WP2_stat2 / 100, 3).ToString                    'dp_Static [mbar]
                TextBox151.Text = Round((WP2_total2 - WP2_stat2) / 100, 3).ToString     'dynamic press [mbar]

                '---------- as vermogen 2e bedrijfspunt -----------------------
                'Direct_as_kw = Round(Scale_rule_Power(T_Power_opt, T_diaw_m, Direct_diaw_m_R20, T_Toerental_rpm, Direct_Toerental_rpm, T_sg_gewicht, G_density_act_zuig), 0)
                Direct_as_kw = 0.001 * Direct_Debiet_z_sec * G_Ptot_Pa / Direct_eff 'Go to kW


                '---------- temperaturen, lost power is tranferred to heat -----------
                Direct_temp_uit_c = G_air_temp + (Direct_as_kw / (cp_air * G_Debiet_kg_s))

                '---------- presenteren-----------------------
                TextBox75.Text = Round(Direct_eff, 3).ToString
                TextBox77.Text = Round((Direct_reynolds * 10 ^ -6), 2).ToString
                TextBox78.Text = Round(Direct_as_kw, 0).ToString                            'As vermogen in [kW]
                TextBox54.Text = Round(Direct_temp_uit_c, 0).ToString                       'Temp uit
                TextBox83.Text = Round(Direct_Debiet_z_sec * 3600.0, 0)                     'Debiet m3/hr

                '--------------------------------------------------------------------------------------------------


                TabPage1.Update()
            Catch ex As Exception
                MessageBox.Show(ex.Message)  ' Show the exception's message.
            End Try
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click, NumericUpDown19.ValueChanged, NumericUpDown17.ValueChanged, NumericUpDown14.ValueChanged, TextBox34.TextChanged, TabPage2.Enter, NumericUpDown20.ValueChanged, NumericUpDown21.ValueChanged, NumericUpDown32.ValueChanged, NumericUpDown31.ValueChanged, ComboBox6.SelectedIndexChanged
        Calc_Stress_1()
    End Sub

    Private Sub Calc_Stress_1()
        Dim maxrpm As Double
        Dim maxV As Double
        Dim sigma_allowed As Double
        Dim sg_staal As Double
        Dim Waaier_dia, Waaier_dik, Waaier_gewicht As Double    'Zonder naaf
        Dim labyrinth_gewicht As Double
        Dim S_breed As Double
        Dim S_dik, S_hoek, S_lengte As Double
        Dim schoep_gewicht As Double
        Dim schoepen_gewicht As Double
        Dim aantal_schoep As Double
        Dim Bodem_gewicht As Double
        Dim Voorplaat_dik As Double
        Dim Voorplaat_gewicht As Double
        Dim sg_ver_gewicht As Double    'sg vervangend gewicht
        Dim sigma_schoep As Double
        Dim sigma_bodemplaat As Double
        Dim V_omtrek As Double
        Dim n_actual As Double
        Dim Voorplaat_keel As Double
        Dim J1, J2, J3, J4, J_tot, I_power, aanlooptijd As Double

        If S_hoek > 90 Then TextBox37.Text = "90"

        Double.TryParse(TextBox34.Text, sigma_allowed)
        sigma_allowed *= 0.7                        'Max 70% van sigma 0.2 (info Peter de Wildt)
        TextBox40.Text = Round(sigma_allowed, 0).ToString
        sigma_allowed *= 1000 ^ 2                 '[N/m2] niet [N/mm2] 

        Double.TryParse(TextBox33.Text, sg_staal)

        Waaier_dia = NumericUpDown21.Value / 1000 '[m]
        Waaier_dik = NumericUpDown17.Value / 1000 '[m]
        Voorplaat_dik = NumericUpDown31.Value / 1000

        '--------Gewichten------------
        If ComboBox1.SelectedIndex > -1 Then '------- schoepgewicht berekenen-----------
            Label128.Text = "Waaier type" & Tschets(ComboBox1.SelectedIndex).Tname
            Label47.Text = "Waaier type" & Tschets(ComboBox1.SelectedIndex).Tname
            Label152.Text = "Waaier type" & Tschets(ComboBox1.SelectedIndex).Tname
            aantal_schoep = Tschets(ComboBox1.SelectedIndex).Tdata(13)
            Voorplaat_keel = Tschets(ComboBox1.SelectedIndex).Tdata(16) / 1000 * (Waaier_dia / 1.0)     '[m]
            S_hoek = Tschets(ComboBox1.SelectedIndex).Tdata(19)                                        'Uittrede hoek in graden
            S_dik = NumericUpDown20.Value / 1000 '[m]
            S_lengte = Tschets(ComboBox1.SelectedIndex).Tdata(12) / 1000 * (Waaier_dia / 1.0)
            S_breed = Tschets(ComboBox1.SelectedIndex).Tdata(15) / 1000 * (Waaier_dia / 1.0)            'Schoep breed uittrede [m]
            schoep_gewicht = S_lengte * S_breed * S_dik * sg_staal
        End If
        Bodem_gewicht = PI / 4 * Waaier_dia ^ 2 * Waaier_dik * sg_staal                                 'Bodem gewicht
        Voorplaat_gewicht = PI / 4 * (Waaier_dia ^ 2 - Voorplaat_keel ^ 2) * Voorplaat_dik * sg_staal   'Voorplaat gewicht (zuig gat verwaarloosd)
        labyrinth_gewicht = NumericUpDown32.Value                                                       'Labyrinth
        schoepen_gewicht = aantal_schoep * schoep_gewicht
        Waaier_gewicht = Bodem_gewicht + schoepen_gewicht + labyrinth_gewicht + Voorplaat_gewicht       'totaal gewicht

        '--------max toerental (beide zijden ingeklemd)-----------
        maxrpm = 0.32 * Sqrt(sigma_allowed * S_dik / (sg_staal * Waaier_dia * S_breed ^ 2 * Cos(S_hoek * PI / 180)))

        '--------max omtreksnelheid------------
        maxV = Sqrt(sigma_allowed * S_dik * Waaier_dia / (sg_staal * S_breed ^ 2 * Cos(S_hoek * PI / 180)))

        '--------vervangen soortelijk gewicht------------
        sg_ver_gewicht = sg_staal * (Bodem_gewicht + (schoep_gewicht * aantal_schoep)) / Bodem_gewicht

        '--------omtrek snelheid------------
        n_actual = NumericUpDown19.Value / 60.0
        V_omtrek = Waaier_dia * PI * n_actual

        '--------- spanning- bodemplaat formule (6.10) page 193------------
        sigma_bodemplaat = 0.83 * sg_ver_gewicht * V_omtrek ^ 2 / 1000 ^ 2 'Trekstekte in N/m2 niet N/mm2

        '--------Spanning schoep formule (6.1a) page 189----------
        sigma_schoep = (sg_staal / 2) * V_omtrek ^ 2 * S_breed ^ 2 * Cos(S_hoek * PI / 180) / (S_dik * Waaier_dia / 2)
        sigma_schoep /= 1000 ^ 2   'Trekstekte in N/m2 niet N/mm2

        'MessageBox.Show("sg=" & sg_staal.ToString & " snelh= " & V_omtrek.ToString & " breed= " & S_breed.ToString & " dik= " & S_dik.ToString & " dia= " & Waaier_dia.ToString & " sigma= " & sigma_schoep.ToString)

        '------------------ Traagheid (0.5 x m x r2)-----------------
        J1 = 0.5 * Bodem_gewicht * (0.5 * Waaier_dia) ^ 2
        J2 = 0.5 * Voorplaat_gewicht * ((0.5 * Waaier_dia) ^ 2 - (0.5 * Voorplaat_keel) ^ 2)
        J3 = 0.5 * labyrinth_gewicht * (0.5 * Voorplaat_keel) ^ 2
        J4 = 0.5 * schoepen_gewicht * (0.5 * (Waaier_dia + Voorplaat_keel) / 2) ^ 2
        J_tot = J1 + J2 + J3 + J4

        '----------------- aanlooptijd---------------
        If (ComboBox6.SelectedIndex > -1) Then      'Prevent exceptions
            Dim words() As String = emotor(ComboBox6.SelectedIndex).Split(";")
            I_power = words(0) * 1000          'Geinstalleerd vermogen [Watt]
            aanlooptijd = 2 * (4 * J_tot) * (n_actual * 60) ^ 2 / (I_power * 10 ^ 3)
        End If

        '--------Present data------------
        TextBox32.Text = Round(sigma_bodemplaat, 0).ToString
        TextBox36.Text = Round(S_breed * 1000, 1).ToString          'Breedte schoep
        TextBox37.Text = Round(S_hoek, 1).ToString                  'Uittrede hoek in graden
        TextBox42.Text = Round(schoep_gewicht, 1).ToString
        TextBox49.Text = Round(maxV, 0).ToString
        TextBox50.Text = Round(aantal_schoep, 0).ToString
        TextBox51.Text = Round(V_omtrek, 0).ToString
        TextBox44.Text = Round(sg_ver_gewicht, 0).ToString
        TextBox43.Text = Round(sigma_schoep, 0).ToString
        TextBox38.Text = Round(maxrpm * 60, 0).ToString
        TextBox45.Text = Round(Bodem_gewicht, 1).ToString
        TextBox94.Text = Round(Voorplaat_gewicht, 1).ToString
        TextBox95.Text = Round(schoepen_gewicht, 1).ToString
        NumericUpDown14.Value = Round(Waaier_gewicht, 0).ToString
        TextBox96.Text = Round(Waaier_gewicht, 1).ToString
        TextBox103.Text = Round(Voorplaat_keel * 1000, 0).ToString
        TextBox104.Text = Round(S_lengte * 1000, 0).ToString

        TextBox105.Text = Round(J1, 1).ToString
        TextBox106.Text = Round(J2, 1).ToString
        TextBox107.Text = Round(J3, 1).ToString
        TextBox108.Text = Round(J4, 1).ToString
        TextBox109.Text = Round(J_tot, 1).ToString
        TextBox146.Text = Round(aanlooptijd, 1).ToString        'Aanlooptijd [s]

        '-------------- check schoep stress safety-----------------------
        If sigma_schoep > sigma_allowed / 1000 ^ 2 Then
            TextBox43.BackColor = Color.Red
        Else
            TextBox43.BackColor = Color.LightGreen
        End If

        '-------------- check rpm safety-----------------------
        If maxrpm > n_actual * 60 Then
            NumericUpDown19.BackColor = Color.Red
        Else
            NumericUpDown19.BackColor = Color.LightGreen
        End If
        '-------------- check bodemplaat stress safety-----------------------
        If sigma_bodemplaat > sigma_allowed / 1000 ^ 2 Then
            TextBox32.BackColor = Color.Red
        Else
            TextBox32.BackColor = Color.LightGreen
        End If
    End Sub

    Private Sub TabPage2_TextChanged(sender As Object, e As EventArgs) Handles TabPage2.TextChanged
        ' Calc_Stress_1()
    End Sub
    'Find the waaier diameter in the Renard reeks
    Function find_Renard_R20(getal As Double)
        Dim x1, x2 As Double

        For hh = 0 To 100
            x1 = Renard_R20(hh)
            x2 = Renard_R20(hh + 1)

            If getal > x1 And getal < x2 Then
                Return (x1)
            End If
        Next hh

        Return (00) 'Return zero when somethings goes wrong
    End Function
    'Renard R20 reeks
    Function Renard_R20(getal As Double)
        Dim Ren As Double

        Ren = (10 ^ (getal / 20) / 10)
        Ren = Round(Ren, 2, MidpointRounding.AwayFromZero)
        Return (Ren)
    End Function

    Function kin_visco_air(temp As Double)
        Dim visco As Double

        '--------- Kinmatic viscosity air[m2/s]-----------------------
        ' Formula valid from -200 to +400 celcius------------------
        If temp > 400 Then MessageBox.Show("kin_visco_air(temp) too high")
        If temp < -200 Then MessageBox.Show("kin_visco_air(temp) too low")

        temp = temp + 273.15
        visco = 0.00009 * temp ^ 2 + 0.0351 * temp - 2.9294

        visco = visco * 10 ^ -6
        Return (visco)
    End Function

    Private Sub fill_array_T_schetsen()

        Tschets(0).Tname = "Willi Bohl"
        Tschets(0).Tdata = {400, 4850, 1.2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(0).Teff = {0, 43, 75, 81, 79, 42, 0, 0, 0, 0, 0, 0}                 '[%]
        Tschets(0).Tverm = {4, 5.1, 5.5, 6.1, 6.6, 9.0, 0.0, 0, 0, 0, 0, 0}         '[kW]
        Tschets(0).TPstat = {10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10}        '[Pa]
        Tschets(0).TPtot = {8666, 8542, 8047, 7428, 6809, 3714, 0, 0, 0, 0, 0, 0}   '[Pa]
        Tschets(0).TFlow = {0, 0.255, 0.51, 0.67, 0.766, 1.021, 0, 0, 0, 0, 0, 0}   '[m3/s]
        Tschets(0).werkp_opT = {81.0, 7428, 0, 6.144, 0.67}                         'rendement, P_totaal [Pa], P_statisch [Pa], as_vermogen [kW], debiet[m3/sec]
        Tschets(0).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(0).TFlow_scaled = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}        '[m3/s]
        Tschets(0).TPstat_scaled = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}       'Statische druk
        Tschets(0).TPtot_scaled = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}        'Totale druk
        Tschets(0).Tverm_scaled = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}        'Rendement[%]
        Tschets(0).Teff_scaled = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}         'Vermogen[kW]

        Tschets(1).Tname = "T1A"
        Tschets(1).Tdata = {1000, 1480, 1.205, 605.4, 832.4, 373.0, 6075.7, 702.7, 881.1, 1063.8, 878.9, 1295.1, 364.3, 12, 133.0, 133.0, 524.3, 605.4, 30, 30}
        Tschets(1).Teff = {0.00, 76.5, 79.5, 80.9, 82.52, 83.26, 83.7, 83.42, 82.3, 79.92, 76.2, 71.0}
        Tschets(1).Tverm = {6.1, 18.9, 20.3, 20.9, 21.4, 21.8, 22.2, 22.5, 22.6, 22.6, 22.5, 22.3}
        Tschets(1).TPstat = {3240.3, 3745.1, 3516.2, 3380.0, 3221.5, 3063.0, 2844.7, 2625.1, 2368.0, 2121.5, 1837.3, 1701.9}
        Tschets(1).TPtot = {3240.3, 3835.0, 3638.5, 3522.6, 3387.7, 3254.5, 3068.9, 2882.7, 2661.2, 2450.3, 2208.8, 2000}
        Tschets(1).TFlow = {0.00, 3.83, 4.47, 4.82, 5.21, 5.59, 6.05, 6.48, 6.92, 7.33, 7.79, 8.17}
        Tschets(1).werkp_opT = {83.5, 250, 0, 19.95, 5.0}
        Tschets(1).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(1).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(1).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(1).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(1).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(1).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(2).Tname = "T1B"
        Tschets(2).Tdata = {1000, 1480, 1.205, 679.0, 814.8, 370.4, 5679.0, 685.2, 850.6, 1043.2, 861.7, 1269.1, 416.0, 12, 129.6, 129.6, 596.3, 30, 30, 0}
        Tschets(2).Teff = {0.00, 30.6, 47.0, 59.5, 69.0, 77.0, 81.0, 80.5, 77.0, 69.0, 55.0, 0}
        Tschets(2).Tverm = {7.0, 10.3, 13.6, 16.8, 19.5, 21.6, 23.1, 23.8, 23.5, 22.3, 20.5, 0}
        Tschets(2).TPstat = {3179.6, 3296.0, 3392.6, 3455.4, 3460.0, 3334.4, 3041.7, 2580.4, 1999.7, 1156.9, 567.0, 0}
        Tschets(2).TPtot = {3179.6, 3302.2, 3417.1, 3509.0, 3555.0, 3486.1, 3256.2, 2873.1, 2382.8, 1792.8, 1164.6, 0}
        Tschets(2).TFlow = {0.00, 0.95, 1.9, 2.85, 3.8, 4.75, 5.7, 6.65, 7.6, 8.55, 9.5, 0}
        Tschets(2).werkp_opT = {99, 99, 9, 9, 9}
        Tschets(2).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(2).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(2).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(2).TPtot_scaled = Tschets(0).TPtot_scaled       'Totale druk
        Tschets(2).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(2).Teff_scaled = Tschets(0).Teff_scaled         'Vermogen[kW]

        Tschets(3).Tname = "T1E"
        Tschets(3).Tdata = {1000, 1480, 1.205, 617.3, 814.8, 370.4, 5679.0, 685.2, 866.7, 1045.7, 859.3, 1044.4, 385.2, 12, 129.6, 129.6, 512.3, 592.6, 30, 30}
        Tschets(3).Teff = {0.00, 59.5, 69.0, 72.5, 75.5, 77.2, 78.0, 78.5, 77.5, 74.0, 66.0, 50.0}
        Tschets(3).Tverm = {7.0, 16.8, 19.4, 20.7, 21.5, 22.3, 22.9, 23.2, 23.1, 22.3, 21.2, 19.4}
        Tschets(3).TPstat = {3179.6, 3455.4, 3452.3, 3389.0, 3279.2, 3121.7, 2934.4, 2654.5, 2413.4, 1792.8, 1156.9, 475.0}
        Tschets(3).TPtot = {3179.6, 3509.0, 3555.0, 3510.0, 3432.4, 3306.0, 3148.9, 2912.0, 2704.6, 2175.9, 1639.6, 1072.6}
        Tschets(3).TFlow = {0.00, 2.85, 3.8, 4.28, 4.75, 5.25, 5.7, 6.25, 6.65, 7.6, 8.55, 9.5}
        Tschets(3).werkp_opT = {78.5, 297, 0, 31.6, 6.25}
        Tschets(3).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(3).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(3).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(3).TPtot_scaled = Tschets(0).TPtot_scaled       'Totale druk
        Tschets(3).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(3).Teff_scaled = Tschets(0).Teff_scaled         'Vermogen[kW]

        Tschets(4).Tname = "T12A+"
        Tschets(4).Tdata = {1000, 1480, 1.205, 897.2, 978.8, 489.4, 6107.7, 685.2, 913.5, 1151.7, 901.3, 1390.7, 243.1, 12, 187.6, 187.6, 758.6, 783.0, 21, 30}
        Tschets(4).Teff = {0.00, 69.0, 77.5, 79.64, 81.22, 82.08, 83.0, 82.0, 79.9, 76.0, 70.5, 40.0}
        Tschets(4).Tverm = {10.3, 26.4, 28.7, 29.1, 29.4, 29.3, 29.2, 28.8, 28.1, 27.4, 26.6, 22.9}
        Tschets(4).TPstat = {2325.7, 2766.8, 2491.5, 2387.2, 2261.6, 2111.9, 1938.1, 1729.1, 1497.0, 1237.7, 962.4, 614.8}
        Tschets(4).TPtot = {2325.7, 2878.2, 2670.2, 2591.3, 2494.7, 2376.0, 2235.3, 2064.2, 1871.4, 1654.0, 1422.9, 1221.3}
        Tschets(4).TFlow = {0.00, 6.58, 8.33, 8.9, 9.52, 10.13, 10.75, 11.4, 12.06, 12.72, 13.38, 14.35}
        Tschets(4).werkp_opT = {83.0, 82, 0, 3.3, 2.5}
        Tschets(4).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(4).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(4).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(4).TPtot_scaled = Tschets(0).TPtot_scaled       'Totale druk
        Tschets(4).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(4).Teff_scaled = Tschets(0).Teff_scaled         'Vermogen[kW]

        Tschets(5).Tname = "T16B+"
        Tschets(5).Tdata = {1000, 1480, 1.205, 291.3, 359.2, 184.5, 4708.7, 689.3, 666.0, 728.2, 631.6, 811.2, 469.4, 10, 32.5, 14.6, 240.8, 289.3, 45, 40}
        Tschets(5).Teff = {0.00, 48.0, 64.2, 66.04, 67.82, 69.06, 69.9, 69.24, 67.86, 65.26, 60.4, 20.0}
        Tschets(5).Tverm = {1.1, 2.0, 2.9, 3.1, 3.3, 3.5, 3.7, 3.9, 4.0, 4.1, 4.1, 4.0}
        Tschets(5).TPstat = {3976.8, 4249.0, 4042.1, 3934.2, 3816.3, 3686.6, 3531.1, 3286.1, 2998.5, 2656.9, 2272.5, 591.8}
        Tschets(5).TPtot = {3976.8, 4256.2, 4070.9, 3970.9, 3862.7, 3743.7, 3600.1, 3371.1, 3101.1, 2780.0, 2418.0, 833.5}
        Tschets(5).TFlow = {0.00, 0.23, 0.46, 0.52, 0.59, 0.65, 0.72, 0.8, 0.87, 0.96, 1.04, 1.34}
        Tschets(5).werkp_opT = {69.0, 1522, 0, 44.4, 1.5}
        Tschets(5).Geljon = {0.141944, 6.35969, -2229.46, 0.0001695, 0.201068, -13.27, 0.00148, 0.00671}
        Tschets(5).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(5).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(5).TPtot_scaled = Tschets(0).TPtot_scaled       'Totale druk
        Tschets(5).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(5).Teff_scaled = Tschets(0).Teff_scaled         'Vermogen[kW]

        Tschets(6).Tname = "T17B+"
        Tschets(6).Tdata = {1000, 1480, 1.205, 738.3, 872.5, 402.7, 5704.7, 617.4, 735.6, 974.5, 837.6, 1273.8, 351.7, 12, 134.2, 134.2, 624.2, 644.3, 27, 30}
        Tschets(6).Teff = {0.00, 72.0, 79.45, 81.16, 82.3, 82.91, 83.0, 82.82, 81.98, 80.34, 77.32, 63.0}
        Tschets(6).Tverm = {5.8, 20.9, 23.1, 23.5, 23.9, 24.2, 24.4, 24.3, 24.1, 23.7, 23.1, 21.1}
        Tschets(6).TPstat = {2850.5, 3094.9, 2960.9, 2888.6, 2787.2, 2662.3, 2506.7, 2352.8, 2115.7, 1853.3, 1581.8, 868.7}
        Tschets(6).TPtot = {2850.5, 3198.0, 3125.4, 3072.8, 2993.9, 2892.7, 2763.7, 2634.4, 2432.4, 2207.2, 1972.8, 1348.8}
        Tschets(6).TFlow = {0.00, 4.64, 5.86, 6.21, 6.57, 6.94, 7.33, 7.67, 8.14, 8.6, 9.04, 10.02}
        Tschets(6).werkp_opT = {83.0, 138, 0, 7.4, 3.0}
        Tschets(6).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(6).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(6).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(6).TPtot_scaled = Tschets(0).TPtot_scaled       'Totale druk
        Tschets(6).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(6).Teff_scaled = Tschets(0).Teff_scaled         'Vermogen[kW]

        Tschets(7).Tname = "T20B"
        Tschets(7).Tdata = {1000, 1480, 1.205, 472.4, 570.1, 275.6, 5275.6, 708.7, 749.6, 859.1, 733.9, 1018.9, 433.1, 10, 126.8, 71.7, 389.0, 442.5, 29, 40}
        Tschets(7).Teff = {0.00, 73.5, 76.02, 78.07, 79.49, 79.95, 80.0, 78.77, 76.71, 74.07, 70.24, 66.0}
        Tschets(7).Tverm = {2.3, 10.2, 11.4, 12.5, 13.5, 14.6, 15.6, 16.4, 17.1, 17.7, 18.1, 18.3}
        Tschets(7).TPstat = {3799.1, 4453.1, 4308.6, 4200.2, 4059.4, 3888.2, 3668.3, 3397.3, 3091.9, 2764.6, 2397.8, 2053.5}
        Tschets(7).TPtot = {3799.1, 4520.3, 4401.7, 4321.2, 4214.4, 4081.3, 3906.7, 3685.7, 3435.1, 3167.5, 2869.2, 2589.8}
        Tschets(7).TFlow = {0.00, 1.68, 1.97, 2.25, 2.54, 2.84, 3.16, 3.47, 3.79, 4.1, 4.44, 4.73}
        Tschets(7).werkp_opT = {80, 628, 0, 16.8, 1.6}
        Tschets(7).Geljon = {0.15345, 1.44388, -116.84, 0.00019665, 0.22452, -3.1435, 0.01084, 0.028648}
        Tschets(7).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(7).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(7).TPtot_scaled = Tschets(0).TPtot_scaled       'Totale druk
        Tschets(7).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(7).Teff_scaled = Tschets(0).Teff_scaled         'Vermogen[kW]

        Tschets(8).Tname = "T21E"
        Tschets(8).Tdata = {1000, 1480, 1.205, 755.6, 673.3, 500.0, 5044.4, 622.2, 706.7, 844.4, 736.9, 1073.6, 242.2, 8, 160.0, 124.4, 626.7, 640.0, 35, 59}
        Tschets(8).Teff = {0.00, 58.0, 67.3, 70.26, 72.46, 73.28, 73.5, 72.82, 71.56, 70.16, 68.2, 66.0}
        Tschets(8).Tverm = {6.2, 17.0, 22.7, 24.7, 26.8, 28.6, 30.4, 31.7, 32.8, 33.4, 34.0, 34.6}
        Tschets(8).TPstat = {3199.6, 3596.4, 3329.8, 3186.0, 3002.4, 2797.8, 2554.7, 2311.6, 2048.7, 1831.7, 1624.6, 1413.8}
        Tschets(8).TPtot = {3199.6, 3636.5, 3432.3, 3327.4, 3192.6, 3043.9, 2868.5, 2691.3, 2500.6, 2344.7, 2202.5, 2054.2}
        Tschets(8).TFlow = {0.00, 2.77, 4.43, 5.21, 6.04, 6.87, 7.76, 8.54, 9.31, 9.92, 10.53, 11.09}
        Tschets(8).werkp_opT = {73.5, 232, 0, 5.9, 1.4}
        Tschets(8).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(8).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(8).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(8).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(8).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(8).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(9).Tname = "T21F"
        Tschets(9).Tdata = {1000, 1480, 1.205, 755.6, 673.3, 500.0, 5044.4, 622.2, 706.7, 844.4, 736.9, 1073.6, 242.2, 16, 160.0, 124.4, 626.7, 640.0, 35, 59}
        Tschets(9).Teff = {0.00, 69.0, 70.2, 71.2, 72.8, 73.86, 75.3, 75.02, 73.7, 70.66, 66.7, 64.0}
        Tschets(9).Tverm = {5.2, 27.8, 30.2, 32.2, 34.1, 35.9, 37.2, 38.8, 40.0, 41.4, 42.3, 41.3}
        Tschets(9).TPstat = {3125.2, 3844.5, 3658.5, 3571.6, 3472.4, 3343.5, 3150.0, 2884.6, 2587.0, 2244.7, 1885.0, 1432.1}
        Tschets(9).TPtot = {3125.2, 3974.2, 3818.5, 3765.4, 3706.8, 3622.4, 3486.6, 3289.3, 3066.1, 2804.5, 2525.4, 2195.1}
        Tschets(9).TFlow = {0.00, 4.99, 5.54, 6.1, 6.71, 7.32, 8.04, 8.81, 9.59, 10.37, 11.09, 12.09}
        Tschets(9).werkp_opT = {75.2, 286, 0, 7.1, 1.4}
        Tschets(9).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(9).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(9).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(9).TPtot_scaled = Tschets(0).TPtot_scaled       'Totale druk
        Tschets(9).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(9).Teff_scaled = Tschets(0).Teff_scaled         'Vermogen[kW]

        Tschets(10).Tname = "T22B"
        Tschets(10).Tdata = {1000, 1480, 1.205, 964.9, 905.3, 659.6, 4666.7, 666.7, 731.6, 912.3, 649.1, 1101.8, 243.9, 24, 321.1, 271.9, 800.0, 800.0, 20, 25}
        Tschets(10).Teff = {0.00, 48.6, 59.8, 63.6, 64.9, 69.5, 70.1, 75.2, 72.2, 71.5, 69.3, 64.8}
        Tschets(10).Tverm = {11.8, 20.6, 28.6, 32.8, 35.5, 37.6, 39.9, 41.4, 42.3, 42.5, 42.0, 40.7}
        Tschets(10).TPstat = {2213.0, 2616.0, 2645.0, 2660.0, 2623.0, 2550.0, 2477.0, 2323.0, 2154.0, 1883.0, 1634.0, 1224.0}
        Tschets(10).TPtot = {2213.0, 2638.0, 2711.0, 2755.0, 2741.0, 2704.0, 2660.0, 2550.0, 2418.0, 2191.0, 1979.0, 1649.0}
        Tschets(10).TFlow = {0.00, 3.79, 6.31, 7.57, 8.41, 9.67, 10.52, 12.2, 12.62, 13.88, 14.72, 15.98}
        Tschets(10).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(10).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(10).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(10).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(10).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(10).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(10).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(11).Tname = "T22C"
        Tschets(11).Tdata = {1000, 1480, 1.205, 964.9, 905.3, 659.6, 4666.7, 666.7, 731.6, 912.3, 649.1, 1101.8, 243.9, 12, 321.1, 271.9, 800.0, 800.0, 20, 25}
        Tschets(11).Teff = {0.00, 41.7, 50.9, 59.0, 63.4, 67.1, 70.3, 72.9, 73.9, 75.2, 74.8, 67.7}
        Tschets(11).Tverm = {17.6, 27.5, 33.7, 38.3, 39.5, 40.4, 41.0, 41.1, 41.1, 40.2, 38.7, 35.6}
        Tschets(11).TPstat = {2345.0, 2682.0, 2638.0, 2536.0, 2470.0, 2382.0, 2250.0, 2089.0, 1891.0, 1744.0, 1363.0, 865.0}
        Tschets(11).TPtot = {2345.0, 2726.0, 2719.0, 2689.0, 2645.0, 2580.0, 2492.0, 2374.0, 2220.0, 2052.0, 1810.0, 1363.0}
        Tschets(11).TFlow = {0.00, 4.21, 6.31, 8.41, 9.46, 10.52, 11.57, 12.62, 13.67, 14.72, 15.98, 17.67}
        Tschets(11).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(11).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(11).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(11).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(11).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(11).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(11).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(12).Tname = "T25B"
        Tschets(12).Tdata = {758, 1465, 1.205, 584.0, 650.0, 345.0, 4766.0, 600.0, 665.6, 804.2, 665.0, 990.0, 220.0, 12, 194.0, 122.0, 430.0, 460.0, 30, 31}
        Tschets(12).Teff = {0, 32.6, 49.5, 61.8, 71.5, 78.2, 82.2, 84.8, 85.1, 83.8, 79.4, 73.2}
        Tschets(12).Tverm = {1.8, 2.9, 3.9, 4.8, 5.7, 6.5, 7.2, 7.6, 7.9, 8.0, 7.9, 7.6}
        Tschets(12).TPstat = {1767, 1864, 1884, 1933, 1962, 1946, 1836, 1670, 1474, 1234, 932, 662}
        Tschets(12).TPtot = {1767, 1867, 1895, 1959, 2010, 2021, 1944, 1816, 1666, 1475, 1230, 1010}
        Tschets(12).TFlow = {0.00, 0.5, 1.0, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0, 5.4}
        Tschets(12).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(12).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(12).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(12).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(12).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(12).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(12).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(13).Tname = "T26"
        Tschets(13).Tdata = {1000, 1480, 1.205, 349.9, 438.1, 213.4, 5689.9, 625.9, 694.2, 768.1, 666.4, 885.5, 470.8, 10, 60.7, 28.9, 290.2, 331.4, 40, 40}
        Tschets(13).Teff = {0.00, 50.0, 68.4, 72.74, 75.38, 76.82, 77.5, 76.82, 74.8, 70.98, 65.5, 44.5}
        Tschets(13).Tverm = {1.7, 3.3, 4.6, 5.2, 5.7, 6.3, 6.8, 7.3, 7.7, 8.0, 8.2, 7.9}
        Tschets(13).TPstat = {4166.8, 4268.4, 4146.5, 4032.7, 3892.4, 3711.5, 3481.8, 3191.2, 2855.8, 2449.3, 2022.4, 1341.5}
        Tschets(13).TPtot = {4166.8, 4278.9, 4185.1, 4089.7, 3973.2, 3820.1, 3624.7, 3375.5, 3089.8, 2735.7, 2370.0, 1804.3}
        Tschets(13).TFlow = {0.00, 0.39, 0.76, 0.92, 1.09, 1.27, 1.45, 1.65, 1.86, 2.06, 2.27, 2.62}
        Tschets(13).werkp_opT = {77.5, 179, 0, 1.5, 0.5}
        Tschets(13).Geljon = {0.14663, 1.5415, -408.8, 0.00032837, 0.1665, -4.3091, 0.002516, 0.016904}
        Tschets(13).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(13).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(13).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(13).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(13).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(14).Tname = "T27"
        Tschets(14).Tdata = {1000, 1480, 1.205, 285.7, 347.8, 183.9, 4596.3, 677.0, 648.9, 701.9, 618.6, 792.5, 414.3, 16, 54.7, 23.6, 288.2, 511.8, 44, 60}
        Tschets(14).Teff = {0.00, 71.0, 73.18, 73.97, 74.37, 74.93, 75.01, 74.8, 74.04, 73.32, 72.43, 65.0}
        Tschets(14).Tverm = {1.3, 4.5, 4.9, 5.2, 5.4, 5.7, 6.0, 6.3, 6.5, 6.7, 6.9, 7.6}
        Tschets(14).TPstat = {3884.0, 4507.1, 4382.5, 4341.0, 4287.0, 4206.0, 4112.5, 4008.6, 3892.3, 3755.3, 3638.9, 3055.3}
        Tschets(14).TPtot = {3884.0, 4578.3, 4475.4, 4445.8, 4407.2, 4345.4, 4272.5, 4187.5, 4091.0, 3974.8, 3876.7, 3426.9}
        Tschets(14).TFlow = {0.00, 0.7, 0.8, 0.85, 0.91, 0.98, 1.05, 1.11, 1.17, 1.23, 1.28, 1.6}
        Tschets(14).werkp_opT = {74.0, 1043, 0, 18.7, 1.0}
        Tschets(14).Geljon = {0.16763, 0.12793, -479.67, -0.000030339, 0.27083, -9.9922, 0.0045166, 0.010324}
        Tschets(14).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(14).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(14).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(14).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(14).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(15).Tname = "T28"
        Tschets(15).Tdata = {1000, 1480, 1.205, 477.3, 477.3, 378.8, 4742.4, 643.9, 706.1, 792.4, 643.9, 882.6, 421.2, 8, 234.8, 151.5, 643.9, 369.7, 0, 0}
        Tschets(15).Teff = {0.00, 49.0, 60.0, 62.36, 64.0, 64.86, 65.0, 64.68, 64.0, 62.88, 60.91, 58.0}
        Tschets(15).Tverm = {11.6, 18.2, 24.5, 26.2, 28.2, 30.3, 32.2, 33.9, 36.0, 37.9, 40.1, 43.2}
        Tschets(15).TPstat = {4450.7, 4762.0, 4473.8, 4337.7, 4185.5, 4017.2, 3828.1, 3629.8, 3389.9, 3159.3, 2882.6, 3413.0}
        Tschets(15).TPtot = {4450.7, 4829.7, 4654.5, 4569.8, 4480.6, 4382.7, 4265.4, 4145.2, 3997.3, 3858.3, 3688.0, 3396.9}
        Tschets(15).TFlow = {0.00, 1.93, 3.16, 3.58, 4.04, 4.5, 4.92, 5.34, 5.8, 6.22, 6.68, 7.38}
        Tschets(15).werkp_opT = {65.0, 186, 0, 5.3, 1.4}
        Tschets(15).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(15).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(15).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(15).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(15).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(15).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(16).Tname = "T31A"
        Tschets(16).Tdata = {1000, 1480, 1.205, 770.4, 857.5, 422.2, 6229.6, 791.6, 878.6, 1060.7, 877.3, 1306.1, 403.0, 8, 197.9, 102.9, 567.3, 606.9, 20, 30}
        Tschets(16).Teff = {0.00, 53.02, 62.41, 77.14, 76.39, 80.74, 83.23, 84.42, 83.2, 79.59, 72.52, 61.87}
        Tschets(16).Tverm = {6.3, 13.0, 15.0, 16.9, 18.3, 19.3, 19.9, 20.1, 20.0, 19.6, 18.8, 17.6}
        Tschets(16).TPstat = {3139.1, 3319.7, 3373.8, 3397.4, 3320.6, 3129.5, 2865.9, 2561.2, 2201.5, 1797.2, 1336.2, 844.5}
        Tschets(16).TPtot = {3139.1, 3339.7, 3409.5, 3453.1, 3400.7, 3238.6, 3008.4, 2741.5, 2424.1, 2066.6, 1656.8, 1220.8}
        Tschets(16).TFlow = {0.00, 2.09, 2.78, 3.48, 4.18, 4.87, 5.57, 6.26, 6.96, 7.65, 8.35, 9.05}
        Tschets(16).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(16).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(16).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(16).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(16).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(16).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(16).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(17).Tname = "T31B"
        Tschets(17).Tdata = {1000, 1480, 1.205, 770.4, 857.5, 422.2, 6229.6, 791.6, 878.6, 1060.7, 877.3, 1306.1, 403.0, 8, 213.7, 118.7, 567.3, 606.9, 20, 30}
        Tschets(17).Teff = {0.00, 56.51, 65.14, 72.1, 78.58, 82.71, 84.89, 85.58, 85.38, 83.38, 80.38, 73.98}
        Tschets(17).Tverm = {6.8, 15.4, 17.2, 18.9, 20.4, 21.6, 22.4, 22.9, 23.0, 23.0, 22.6, 22.2}
        Tschets(17).TPstat = {3156.6, 3426.6, 3439.5, 3439.1, 3408.2, 3269.9, 3024.2, 2742.8, 2432.4, 2094.9, 1735.6, 1333.4}
        Tschets(17).TPtot = {3156.6, 3456.6, 3487.9, 3510.6, 3507.1, 3400.7, 3191.5, 2950.8, 2685.7, 2398.0, 2092.8, 1749.2}
        Tschets(17).TFlow = {0.00, 2.55, 3.25, 3.94, 4.64, 5.34, 6.03, 6.73, 7.42, 8.12, 8.81, 9.51}
        Tschets(17).werkp_opT = {87.7, 161, 0, 7.3, 3.0}
        Tschets(17).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(17).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(17).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(17).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(17).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(17).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(18).Tname = "T31C+"
        Tschets(18).Tdata = {1000, 1480, 1.205, 770.4, 857.5, 455.1, 6229.6, 791.6, 878.6, 1060.7, 877.3, 1306.1, 403.0, 8, 246.7, 151.7, 567.3, 606.9, 20, 30}
        Tschets(18).Teff = {0.00, 62.34, 69.36, 75.3, 80.45, 84.07, 85.99, 86.61, 85.9, 83.74, 81.3, 77.49}
        Tschets(18).Tverm = {7.9, 18.7, 20.6, 22.4, 23.8, 24.9, 25.9, 26.7, 27.4, 27.9, 27.9, 27.5}
        Tschets(18).TPstat = {3231.6, 3591.0, 3607.8, 3594.7, 3528.8, 3380.7, 3174.7, 2943.9, 2674.4, 2374.9, 2055.9, 1703.4}
        Tschets(18).TPtot = {3231.6, 3632.7, 3669.3, 3679.8, 3641.4, 3524.6, 3353.7, 3161.8, 2935.1, 2682.2, 2413.7, 2115.4}
        Tschets(18).TFlow = {0.00, 3.25, 3.94, 4.64, 5.34, 6.03, 6.73, 7.42, 8.12, 8.81, 9.51, 10.21}
        Tschets(18).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(18).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(18).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(18).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(18).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(18).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(18).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(19).Tname = "T31D"
        Tschets(19).Tdata = {1000, 1480, 1.205, 770.4, 857.5, 521.1, 6229.6, 791.6, 878.6, 1060.7, 877.3, 1306.1, 403.0, 8, 278.4, 183.4, 567.3, 606.9, 20, 30}
        Tschets(19).Teff = {0.00, 48.55, 59.16, 67.74, 74.91, 80.69, 84.59, 86.61, 86.83, 84.66, 80.56, 74.37}
        Tschets(19).Tverm = {8.5, 17.0, 20.2, 23.0, 25.5, 27.5, 29.2, 30.6, 31.6, 32.4, 32.6, 32.3}
        Tschets(19).TPstat = {3351.9, 3597.3, 3689.8, 3735.3, 3709.3, 3622.4, 3446.5, 3213.1, 2922.2, 2570.2, 2152.1, 1692.1}
        Tschets(19).TPtot = {3351.9, 3613.5, 3721.6, 3787.9, 3787.9, 3732.1, 3592.6, 3400.7, 3156.6, 2856.6, 2495.6, 2098.0}
        Tschets(19).TFlow = {0.00, 2.32, 3.25, 4.18, 5.1, 6.03, 6.96, 7.89, 8.81, 9.74, 10.67, 11.6}
        Tschets(19).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(19).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(19).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(19).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(19).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(19).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(19).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(20).Tname = "T31E"
        Tschets(20).Tdata = {1000, 1480, 1.205, 770.4, 857.5, 521.1, 6229.6, 791.6, 878.6, 1060.7, 877.3, 1306.1, 403.0, 8, 310.0, 215.0, 567.3, 606.9, 20, 30}
        Tschets(20).Teff = {0.00, 55.18, 64.4, 71.52, 77.81, 82.51, 84.57, 85.39, 84.76, 82.84, 79.16, 73.67}
        Tschets(20).Tverm = {10.0, 20.2, 23.1, 25.9, 28.3, 30.6, 32.7, 34.6, 35.9, 36.9, 37.3, 37.0}
        Tschets(20).TPstat = {3435.6, 3722.1, 3781.1, 3789.6, 3752.7, 3661.8, 3485.5, 3262.2, 2974.4, 2636.0, 2241.9, 1793.7}
        Tschets(20).TPtot = {3435.6, 3749.5, 3828.0, 3861.2, 3854.2, 3798.4, 3662.3, 3484.5, 3247.3, 2964.8, 2631.7, 2249.7}
        Tschets(20).TFlow = {0.00, 3.02, 3.94, 4.87, 5.8, 6.73, 7.65, 8.58, 9.51, 10.44, 11.37, 12.29}
        Tschets(20).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(20).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(20).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(20).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(20).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(20).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(20).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(21).Tname = "T33B"
        Tschets(21).Tdata = {1000, 1480, 1.205, 1013.2, 857.5, 637.2, 6229.6, 791.6, 877.3, 1060.7, 877.3, 1306.1, 411.6, 8, 314.0, 233.5, 659.6, 688.7, 10, 30}
        Tschets(21).Teff = {0.00, 64.0, 85.1, 86.98, 88.16, 89.0, 89.28, 88.52, 87.1, 84.2, 80.5, 74.0}
        Tschets(21).Tverm = {10.6, 24.3, 29.9, 30.4, 30.9, 31.2, 31.4, 31.0, 30.4, 29.5, 28.2, 26.2}
        Tschets(21).TPstat = {2902.2, 3304.3, 3007.1, 2888.2, 2725.6, 2578.8, 2355.0, 2073.5, 1835.7, 1521.0, 1223.8, 786.7}
        Tschets(21).TPtot = {2902.2, 3346.9, 3137.5, 3038.7, 2901.1, 2775.6, 2582.2, 2341.7, 2137.0, 1864.4, 1607.0, 1236.4}
        Tschets(21).TFlow = {0.00, 4.64, 8.12, 8.72, 9.42, 9.97, 10.72, 11.64, 12.34, 13.18, 13.92, 15.08}
        Tschets(21).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(21).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(21).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(21).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(21).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(21).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(21).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(22).Tname = "T34"
        Tschets(22).Tdata = {1000, 1480, 1.205, 1013.2, 857.5, 693.9, 6229.6, 791.6, 877.3, 1060.7, 877.3, 1306.1, 411.6, 8, 370.7, 290.2, 659.6, 688.7, 10, 30}
        Tschets(22).Teff = {0.00, 69.0, 83.2, 85.2, 87.0, 88.22, 88.9, 88.3, 87.12, 85.1, 82.6, 52.0}
        Tschets(22).Tverm = {12.2, 28.6, 33.8, 34.7, 35.2, 35.7, 35.9, 35.9, 35.4, 34.7, 33.9, 26.6}
        Tschets(22).TPstat = {2954.7, 3339.3, 2874.2, 2788.6, 2694.8, 2535.7, 2372.5, 2173.2, 1963.4, 1739.6, 1503.6, 760.5}
        Tschets(22).TPtot = {2954.7, 3400.0, 3017.8, 2954.5, 2886.3, 2754.1, 2620.8, 2452.6, 2273.6, 2084.5, 1882.7, 1334.8}
        Tschets(22).TFlow = {0.00, 6.03, 9.28, 9.97, 10.72, 11.46, 12.2, 12.94, 13.64, 14.38, 15.08, 18.56}
        Tschets(22).werkp_opT = {88.5, 157, 0, 11.8, 5.0}
        Tschets(22).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(22).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(22).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(22).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(22).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(22).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(23).Tname = "T35A"
        Tschets(23).Tdata = {1000, 1480, 1.205, 233.3, 333.3, 175.0, 5016.7, 792.0, 736.7, 760.0, 650.0, 816.7, 433.3, 8, 108.3, 33.3, 375.0, 133.3, 0, 0}
        Tschets(23).Teff = {0.00, 41.0, 53.0, 54.89, 56.02, 56.56, 56.9, 56.44, 55.9, 54.8, 52.56, 36.5}
        Tschets(23).Tverm = {3.4, 5.0, 6.7, 7.2, 7.9, 8.4, 9.1, 9.7, 10.4, 11.0, 11.8, 14.6}
        Tschets(23).TPstat = {4325.0, 4499.4, 4206.4, 4080.9, 3913.5, 3744.6, 3536.8, 3313.5, 3048.5, 2787.6, 2419.2, 2232.3}
        Tschets(23).TPtot = {4325.0, 4530.2, 4322.7, 4232.7, 4114.3, 3996.4, 3850.6, 3696.2, 3513.5, 3335.8, 3088.9, 2380.7}
        Tschets(23).TFlow = {0.00, 0.42, 0.82, 0.94, 1.08, 1.2, 1.34, 1.48, 1.64, 1.78, 1.96, 2.57}
        Tschets(23).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(23).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(23).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(23).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(23).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(23).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(23).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(24).Tname = "T35B"
        Tschets(24).Tdata = {1000, 1480, 1.205, 233.3, 333.3, 175.0, 5016.7, 792.0, 736.7, 760.0, 650.0, 816.7, 433.3, 8, 108.3, 50.0, 375.0, 133.3, 0, 0}
        Tschets(24).Teff = {0.00, 40.0, 50.0, 55.06, 57.72, 59.1, 59.6, 56.6, 52.0, 45.0, 35.0, 31.0}
        Tschets(24).Tverm = {3.4, 5.6, 6.6, 7.5, 8.5, 9.5, 10.5, 12.5, 14.0, 15.7, 17.5, 18.2}
        Tschets(24).TPstat = {4429.7, 4729.6, 4604.1, 4459.0, 4234.3, 3981.8, 3683.3, 2949.4, 2346.7, 1576.5, 558.1, 0.0}
        Tschets(24).TPtot = {4429.7, 4767.6, 4689.5, 4596.0, 4444.0, 4279.5, 4084.2, 3603.2, 3221.4, 2725.0, 2041.0, 1522.3}
        Tschets(24).TFlow = {0.00, 0.47, 0.7, 0.89, 1.1, 1.31, 1.52, 1.94, 2.24, 2.57, 2.92, 3.04}
        Tschets(24).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(24).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(24).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(24).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(24).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(24).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(24).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(25).Tname = "T35D"  'CHECH TORERENTAL
        Tschets(25).Tdata = {1000, 1480, 1.205, 303.3, 333.3, 175.0, 5016.7, 1208.3, 736.7, 760.0, 650.0, 816.7, 433.3, 8, 108.3, 50.0, 375.0, 133.3, 0, 0}
        Tschets(25).Teff = {0.00, 30.0, 45.5, 47.98, 50.7, 51.92, 52.1, 51.68, 50.14, 47.48, 44.0, 30.0}
        Tschets(25).Tverm = {5.0, 7.1, 9.3, 9.9, 10.9, 11.7, 12.8, 14.2, 15.6, 17.2, 18.7, 22.4}
        Tschets(25).TPstat = {4813.3, 4743.6, 4429.7, 4302.7, 4107.4, 3919.0, 3655.4, 3278.7, 2846.2, 2323.2, 1744.0, 154.8}
        Tschets(25).TPtot = {4813.3, 4781.5, 4581.5, 4503.5, 4384.1, 4272.2, 4120.4, 3901.4, 3649.5, 3339.7, 2999.1, 2016.0}
        Tschets(25).TFlow = {0.00, 0.47, 0.94, 1.08, 1.26, 1.43, 1.64, 1.89, 2.15, 2.43, 2.69, 3.27}
        Tschets(25).werkp_opT = {53.0, 600, 0, 10.5, 0.7}
        Tschets(25).Geljon = {0, 0, 0, 0, 0, 0, 0, 0}
        Tschets(25).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(25).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(25).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(25).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(25).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(26).Tname = "T36"
        Tschets(26).Tdata = {1000, 1480, 1.205, 523.7, 572.4, 328.9, 5657.9, 710.5, 750.0, 859.2, 733.6, 1019.7, 425.0, 10, 156.6, 62.5, 442.1, 464.5, 29, 40}
        Tschets(26).Teff = {0.00, 71.5, 80.5, 83.5, 86.52, 87.58, 88.0, 87.68, 86.2, 83.4, 79.0, 57.0}
        Tschets(26).Tverm = {3.0, 9.6, 12.0, 13.1, 14.1, 15.0, 15.9, 16.8, 17.4, 17.8, 18.6, 17.6}
        Tschets(26).TPstat = {3930.4, 4269.4, 4121.7, 4034.8, 3883.5, 3688.7, 3471.3, 3177.4, 2805.4, 2469.6, 2052.2, 1443.5}
        Tschets(26).TPtot = {3930.4, 4312.8, 4210.0, 4151.5, 4034.9, 3879.4, 3705.7, 3469.7, 3206.9, 2896.6, 2560.4, 2237.6}
        Tschets(26).TFlow = {0.00, 1.61, 2.3, 2.65, 3.01, 3.38, 3.75, 4.19, 4.63, 5.06, 5.52, 5.9}
        Tschets(26).werkp_opT = {87.9, 221, 0, 5.1, 1.5}
        Tschets(26).Geljon = {0.14499, 1.2327, -79.528, 0.00060039, 0.16925, -1.9817, 0.01039, 0.03562}
        Tschets(26).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(26).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(26).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(26).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(26).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(27).Tname = "T36A+"
        Tschets(27).Tdata = {1000, 1480, 1.205, 523.7, 572.4, 328.9, 5657.9, 710.5, 750.0, 859.2, 733.6, 1019.7, 425.0, 10, 133.6, 62.5, 442.1, 464.5, 29, 40}
        Tschets(27).Teff = {0.00, 71.0, 80.5, 82.1, 83.3, 84.24, 84.6, 84.04, 81.46, 77.16, 72.0, 60.0}
        Tschets(27).Tverm = {3.0, 9.0, 11.9, 12.7, 13.5, 14.1, 14.8, 15.6, 16.4, 16.9, 17.1, 17.1}
        Tschets(27).TPstat = {3826.1, 4252.2, 4060.9, 3963.5, 3833.9, 3695.7, 3533.9, 3220.0, 2868.7, 2440.9, 2036.5, 1756.5}
        Tschets(27).TPtot = {3826.1, 4289.5, 4149.1, 4072.2, 3967.4, 3854.1, 3719.4, 3457.3, 3164.2, 2804.5, 2463.6, 2308.0}
        Tschets(27).TFlow = {0.00, 1.5, 2.3, 2.55, 2.83, 3.08, 3.34, 3.77, 4.21, 4.67, 5.06, 5.75}
        Tschets(27).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(27).Geljon = {0.14412, 1.3974, -98.38, 0.00040028, 0.19121, -2.6461, 0.009678, 0.032648}
        Tschets(27).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(27).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(27).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(27).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(27).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(28).Tname = "GALAK"
        Tschets(28).Tdata = {1200, 1465, 1.2, 500, 665, 300, 5500, 820, 886, 1018, 875, 1208, 408, 16, 120.0, 60.0, 455, 500, 38.4, 71.0}
        Tschets(28).Teff = {0.00, 33.1, 52.7, 66.2, 72.2, 74.1, 75.0, 75.1, 74.3, 72.9, 70.5, 63.4}
        Tschets(28).Tverm = {8.5, 15.3, 22.1, 29.1, 35.8, 38.8, 42.0, 45.1, 48.4, 51.5, 54.5, 56.9}
        Tschets(28).TPstat = {5258.2, 5655.1, 5982.7, 6182.0, 6174.5, 6022.2, 5832.8, 5586.9, 5284.4, 4925.3, 4529.3, 4057.0}
        Tschets(28).TPtot = {5258.2, 5670.2, 6043.0, 6317.6, 6415.7, 6327.5, 6209.7, 6043.0, 5827.1, 5562.3, 5268.0, 4905.0}
        Tschets(28).TFlow = {0.00, 1.0, 2.0, 3.0, 4.0, 4.5, 5.0, 5.5, 6.0, 6.5, 7.0, 7.5}
        Tschets(28).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(28).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(28).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(28).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(28).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(28).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(29).Tname = "GW"
        Tschets(29).Tdata = {1000, 1480, 1.205, 127.8, 156.7, 82.5, 4117.5, 618.6, 573.2, 573.2, 536.1, 614.4, 416.5, 12, 20.6, 8.2, 127.8, 167.0, 65, 90}
        Tschets(29).Teff = {0.00, 27.8, 40.2, 43.9, 46.4, 47.5, 47.5, 46.6, 43.8, 39.9, 34.8, 28.3}
        Tschets(29).Tverm = {0.4, 0.7, 1.0, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0}
        Tschets(29).TPstat = {3953.0, 4082.0, 4030.0, 3936.0, 3804.0, 3630.0, 3405.0, 3141.0, 2793.0, 2414.0, 1956.0, 1452.0}
        Tschets(29).TPtot = {3953.0, 4089.0, 4058.0, 3978.0, 3863.0, 3710.0, 3509.0, 3273.0, 2953.0, 2595.0, 2189.0, 1727.0}
        Tschets(29).TFlow = {0.00, 0.05, 0.1, 0.13, 0.15, 0.18, 0.2, 0.23, 0.25, 0.28, 0.3, 0.33}
        Tschets(29).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(29).Geljon = {0.1327, 32.1, -31159.0, 0.0001441, 0.26841, -23.368, 0.0004895, 0.001958}
        Tschets(29).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(29).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(29).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(29).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(29).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

        Tschets(30).Tname = "GWA"
        Tschets(30).Tdata = {1000, 1480, 1.205, 127.8, 156.7, 82.5, 4117.5, 618.6, 1191.8, 573.2, 536.1, 577.3, 492.1, 12, 20.6, 8.2, 127.8, 167.0, 90, 50}
        Tschets(30).Teff = {0.00, 30.27, 38.83, 44.99, 49.14, 51.21, 52.45, 52.1, 50.65, 47.89, 43.59, 38.33}
        Tschets(30).Tverm = {0.4, 0.7, 0.8, 0.9, 1.0, 1.1, 1.2, 1.2, 1.3, 1.4, 1.5, 1.6}
        Tschets(30).TPstat = {3880.8, 3918.9, 3880.8, 3794.0, 3672.5, 3519.8, 3335.8, 3096.3, 2804.7, 2454.1, 2020.2, 1600.2}
        Tschets(30).TPtot = {3880.8, 3936.3, 3905.1, 3832.2, 3731.5, 3596.1, 3429.5, 3217.8, 2971.3, 2690.1, 2350.0, 1985.5}
        Tschets(30).TFlow = {0.00, 0.05, 0.08, 0.1, 0.13, 0.15, 0.18, 0.2, 0.23, 0.25, 0.28, 0.3}
        Tschets(30).werkp_opT = {0, 0, 0, 0, 0}
        Tschets(30).Geljon = {0.13513, 25.537, -28003.0, 0.0000681, 0.21049, -33.199, 0.000566, 0.001838}
        Tschets(30).TFlow_scaled = Tschets(0).TFlow_scaled        '[m3/s]
        Tschets(30).TPstat_scaled = Tschets(0).TPstat_scaled      'Statische druk
        Tschets(30).TPtot_scaled = Tschets(0).TPtot_scaled        'Totale druk
        Tschets(30).Tverm_scaled = Tschets(0).Tverm_scaled        'Rendement[%]
        Tschets(30).Teff_scaled = Tschets(0).Teff_scaled          'Vermogen[kW]

    End Sub

    Private Sub draw_chart1(Tschets_no As Integer)
        Dim hh As Integer
        Dim debiet As Double
        Dim Q_target, P_target As Double
        Dim Weerstand_coefficient, p_loss_line As Double


        If Tschets_no > 30 Or Tschets_no < 0 Then
            MessageBox.Show("Problem in line 1225")
        End If

        'Gewenste fan  gegevens
        P_target = NumericUpDown2.Value * 100   '[Pa]

        Try
            'Clear all series And chart areas so we can re-add them
            Chart1.Series.Clear()
            Chart1.ChartAreas.Clear()
            Chart1.Titles.Clear()

            Chart1.Series.Add("Series0")    'Pressure totaal
            Chart1.Series.Add("Series1")    'Efficiency
            Chart1.Series.Add("Series2")    'Power
            Chart1.Series.Add("Series3")    'Market dot
            Chart1.Series.Add("Series4")    'Line resistance(Future use)

            Chart1.ChartAreas.Add("ChartArea0")
            Chart1.Series(0).ChartArea = "ChartArea0"
            Chart1.Series(1).ChartArea = "ChartArea0"
            Chart1.Series(2).ChartArea = "ChartArea0"

            If CheckBox1.Checked = False Then
                Chart1.Series(0).ChartType = DataVisualization.Charting.SeriesChartType.Line
                Chart1.Series(1).ChartType = DataVisualization.Charting.SeriesChartType.Line
                Chart1.Series(2).ChartType = DataVisualization.Charting.SeriesChartType.Line
                Chart1.Series(3).ChartType = DataVisualization.Charting.SeriesChartType.Line
            Else
                Chart1.Series(0).ChartType = DataVisualization.Charting.SeriesChartType.Spline
                Chart1.Series(1).ChartType = DataVisualization.Charting.SeriesChartType.Spline
                Chart1.Series(2).ChartType = DataVisualization.Charting.SeriesChartType.Spline
                Chart1.Series(3).ChartType = DataVisualization.Charting.SeriesChartType.Spline
            End If
            Chart1.Series(4).ChartType = DataVisualization.Charting.SeriesChartType.Line

            Chart1.Titles.Add(Tschets(Tschets_no).Tname)
            Chart1.Titles(0).Font = New Font("Arial", 16, System.Drawing.FontStyle.Bold)

            Chart1.Series(0).Name = "P totaal [Pa]"
            Chart1.Series(1).Name = "Rendement [%]"
            Chart1.Series(2).Name = "As vermogen [kW]"
            Chart1.Series(3).Name = "Line resistance"

            Chart1.Series(0).Color = Color.Blue
            Chart1.Series(1).Color = Color.Red
            Chart1.Series(2).Color = Color.Green
            Chart1.Series(3).Color = Color.Blue

            Chart1.Series(0).IsValueShownAsLabel = True
            Chart1.Series(1).IsValueShownAsLabel = True
            Chart1.Series(2).IsValueShownAsLabel = True

            Chart1.Series(0).BorderWidth = 4
            Chart1.Series(1).BorderWidth = 3
            Chart1.Series(2).BorderWidth = 4

            Chart1.ChartAreas("ChartArea0").AxisX.Minimum = 0
            Chart1.ChartAreas("ChartArea0").AxisX.MinorTickMark.Enabled = True
            Chart1.ChartAreas("ChartArea0").AxisY.MinorTickMark.Enabled = True
            Chart1.ChartAreas("ChartArea0").AxisY2.MinorTickMark.Enabled = True

            '---------------- fan target ---------------------
            TextBox149.Text = Round(G_Debiet_z_act_hr, 0).ToString  'Debiet [Am3/hr]
            TextBox148.Text = NumericUpDown2.Value.ToString         'Ptotal [mbar]

            If CheckBox2.Checked = False Then
                Chart1.ChartAreas("ChartArea0").AxisX.Title = "Debiet [Am3/s]"
            Else
                Chart1.ChartAreas("ChartArea0").AxisX.Title = "Debiet [Am3/hr]"
            End If

            Chart1.ChartAreas("ChartArea0").AxisY.Title = "Ptotaal [Pa]"
            Chart1.ChartAreas("ChartArea0").AlignmentOrientation = DataVisualization.Charting.AreaAlignmentOrientations.Vertical
            Chart1.ChartAreas("ChartArea0").AxisY2.Enabled = AxisEnabled.True
            Chart1.ChartAreas("ChartArea0").AxisY2.Title = "Rendement [%] As-vermogen [kW]"

            If CheckBox2.Checked = True Then
                Chart1.ChartAreas("ChartArea0").AxisX.Title = "Debiet [Am3/s]"
                Q_target = G_Debiet_z_act_hr            '[Am3/hr]
            Else
                Chart1.ChartAreas("ChartArea0").AxisX.Title = "Debiet [Am3/hr]"
                Q_target = G_Debiet_z_act_hr / 3600     '[Am3/sec]
            End If


            '------------------Weerstand coefficient ---------------------
            If CheckBox2.Checked = True Then
                Weerstand_coefficient = P_target * 2 / (NumericUpDown11.Value * (G_Debiet_z_act_hr) ^ 2)
            Else
                Weerstand_coefficient = P_target * 2 / (NumericUpDown11.Value * (G_Debiet_z_act_hr / 3600) ^ 2)
            End If


            '-------------------Target dot ---------------------
            If CheckBox3.Checked = True Then
                Chart1.Series(3).YAxisType = AxisType.Primary
                Chart1.Series(3).Points.AddXY(Q_target, P_target)
                Chart1.Series(3).Points(0).MarkerStyle = DataVisualization.Charting.MarkerStyle.Star10
                Chart1.Series(3).Points(0).MarkerSize = 20
            End If

            '-------------------'P_totaal lijn [mmwc]---------------------
            For hh = 0 To 11   'Fill line chart
                If Tschets(Tschets_no).TPtot(hh) > 0 Then           'Rest of the array is empty
                    If CheckBox2.Checked = True Then
                        debiet = Round(Tschets(Tschets_no).TFlow_scaled(hh) * 3600, 0)
                    Else
                        debiet = Tschets(Tschets_no).TFlow_scaled(hh)
                    End If

                    Chart1.Series(0).YAxisType = AxisType.Primary
                    Chart1.Series(0).Points.AddXY(debiet, Tschets(Tschets_no).TPtot_scaled(hh))
                    Chart1.Series(1).YAxisType = AxisType.Secondary
                    Chart1.Series(1).Points.AddXY(debiet, Tschets(Tschets_no).Teff_scaled(hh))
                    Chart1.Series(2).YAxisType = AxisType.Secondary
                    Chart1.Series(2).Points.AddXY(debiet, Tschets(Tschets_no).Tverm_scaled(hh))
                    If CheckBox3.Checked = True Then
                        p_loss_line = 0.5 * Weerstand_coefficient * NumericUpDown11.Value * debiet ^ 2
                        Chart1.Series(4).Points.AddXY(debiet, p_loss_line)
                    End If
                End If
            Next hh
            Chart1.Refresh()
        Catch ex As Exception
            'MessageBox.Show(ex.Message & "Line 1100")  ' Show the exception's message.
        End Try
    End Sub

    'Save data and line chart to file
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        save_to_disk()
    End Sub
    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        Selectie_1()
    End Sub
    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        Selectie_1()
    End Sub
    'Calculatie soortelijk gewicht
    'Zonder vochtigheidcompensatie
    Private Function calc_sg_air(P As Double, T As Double, RH As Double, MG As Double)
        Dim p1, p2, sg1, sg2 As Double

        If RadioButton1.Checked = True Then 'Medium is Lucht
            NumericUpDown8.Value = 28.96    'According ISO6972 for dry air
            NumericUpDown8.BackColor = Color.White
        Else
            NumericUpDown8.BackColor = Color.Yellow 'Medium is gas
        End If

        '---------------- We assume that above the 100c the air is dry ----------------------------
        '--------------- otherwise unpredictable results-------------------------------------------
        If T > 100 Then
            NumericUpDown5.Value = 0
        End If

        '--------------------------------Partiele waterdamp druk----------------------------
        If T >= 0 And T <= 99 Then
            p1 = Pow(10, (8.07131 - (1730.63 / (233.462 + T)))) * 133.322368
        End If

        If T >= 100 And T < 374 Then
            p1 = Pow(10, (8.14019 - (1810.94 / (244.485 + T)))) * 133.322368
        End If


        p1 = p1 * RH / 100
        p2 = P - p1

        'MessageBox.Show("p1= " & p1.ToString & " p2= " & p2.ToString)

        '-------------------------------- soortelijk gewicht---------------------------
        'gecontroleerd tegen http://www.denysschen.com/catalogue/density.aspx --------
        'Algmene Gasconstante is 8314,32
        '8314,32/28.8= 288.69

        sg1 = p1 / (461.495 * (T + 273.15))             'Water vapor
        sg2 = p2 / (8.31432 / MG * (T + 273.15))        'Droge lucht

        Return (sg1 + sg2)
    End Function
    'Save data and line chart to file
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        save_to_disk()
    End Sub
    'Find workpoint hight efficiency
    Private Sub Find_hi_eff()
        Dim hh, jj, pos_counter As Integer
        Dim eff_hi As Double

        For jj = 0 To (UBound(Tschets) - 1)                '30 T schetsen 
            eff_hi = 0
            pos_counter = 0
            For hh = 0 To 11                'Check all Efficiencies to find the highest
                If Tschets(jj).Teff(hh) > eff_hi Then
                    eff_hi = Tschets(jj).Teff(hh)
                    pos_counter = hh
                End If
            Next hh

            Tschets(jj).werkp_opT(0) = Tschets(jj).Teff(pos_counter)    'rendement
            Tschets(jj).werkp_opT(1) = Tschets(jj).TPtot(pos_counter)   'P_totaal [Pa]
            Tschets(jj).werkp_opT(2) = Tschets(jj).TPstat(pos_counter)  'P_statisch [Pa]
            Tschets(jj).werkp_opT(3) = Tschets(jj).Tverm(pos_counter)   'as_vermogen [kW]
            Tschets(jj).werkp_opT(4) = Tschets(jj).TFlow(pos_counter)   'debiet[m3/sec]
            'MessageBox.Show("JJ=" & jj.ToString & " aantal hh =" & hh)
        Next jj
    End Sub

    'Save data and line chart to file
    Private Sub save_to_disk()
        Dim bmp_tab_page1 As New Bitmap(TabPage1.Width, TabPage1.Height)
        Dim bmp_tab_page2 As New Bitmap(TabPage2.Width, TabPage2.Height)
        Dim str_file1, str_file2, str_file3 As String

        Dim text As String
        text = Now.ToString("yyyy_MM_dd_HH_mm_ss_")

        str_file1 = "c:\temp\" & text & "Fan chart.png"
        str_file2 = "c:\temp\" & text & "Fan selection data.png"
        str_file3 = "c:\temp\" & text & "Fan stress waaier.png"

        '---------- save chart---------------
        Chart1.Show()
        Chart1.Refresh()
        Chart1.SaveImage(str_file1, Imaging.ImageFormat.Png)

        '---- save tab page 1---------------
        TabPage2.Show()
        TabPage1.DrawToBitmap(bmp_tab_page1, DisplayRectangle)
        bmp_tab_page1.Save(str_file2, Imaging.ImageFormat.Png)

        '---- save tab page 2---------------
        TabPage2.Show()
        TabPage2.DrawToBitmap(bmp_tab_page2, DisplayRectangle)
        bmp_tab_page2.Save(str_file3, Imaging.ImageFormat.Png)

        MessageBox.Show("Files is saved to c:\temp ")

    End Sub

    'Graphic next model
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If ComboBox1.SelectedIndex < (ComboBox1.Items.Count - 1) Then
            ComboBox1.SelectedIndex += 1
            Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
            draw_chart1(ComboBox1.SelectedIndex)
        End If
    End Sub
    'Graphic previous model
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If (ComboBox1.SelectedIndex > 0) Then
            ComboBox1.SelectedIndex -= 1
            Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
            draw_chart1(ComboBox1.SelectedIndex)
        End If
    End Sub
    'Scale rules capacity
    '1= inlet, 2=outlet
    'Diameter in [m]
    'Q=Capacity in [m3/s]
    'Speed in [rpm] or [rad/s] or [rps] if used consequently
    'Note; sg medum speelt geen rol !!!!!!!!!!
    Private Function Scale_rule_cap(Q1 As Double, Dia1 As Double, Dia2 As Double, n1 As Double, n2 As Double)
        Dim Q2 As Double

        Q2 = Q1 * (n2 / n1) * (Dia2 / Dia1) ^ 3

        Return (Q2)
    End Function

    'Scale rules Pressure, Total and Static
    '1= inlet, 2=outlet
    'Diameter in [m]
    'Q=Capacity in [m3/s]
    'Speed in [rpm] or [rad/s] or [rps] if used consequently
    Private Function Scale_rule_Pressure(Pt1 As Double, Dia1 As Double, Dia2 As Double, n1 As Double, n2 As Double, Ro1 As Double, Ro2 As Double)
        Dim Pt2 As Double

        Pt2 = Pt1 * (n2 / n1) ^ 2 * (Ro2 / Ro1) * (Dia2 / Dia1) ^ 2

        Return (Pt2)
    End Function
    'Scale rules Power
    '1= old, 2= new
    'Diameter in [m]
    'Q=Capacity in [m3/s]
    'Speed in [rpm] or [rad/s] or [rps] if used consequently
    Private Function Scale_rule_Power(Power1 As Double, Dia1 As Double, Dia2 As Double, n1 As Double, n2 As Double, Ro1 As Double, Ro2 As Double)
        Dim Power2 As Double

        Power2 = Power1 * (n2 / n1) ^ 3 * (Ro2 / Ro1) * (Dia2 / Dia1) ^ 5

        Return (Power2)
    End Function

    Private Sub Chart1_Layout(sender As Object, e As LayoutEventArgs) Handles Chart1.Layout
        Dim ww, hh As Integer

        ww = Chart1.Size.Width - 170
        hh = Chart1.Size.Height * 0.1 + 80

        GroupBox36.Location = New Point(ww, hh)
        hh += GroupBox36.Height + 10
        GroupBox37.Location = New Point(ww, hh)
        hh += GroupBox37.Height + 10
        GroupBox39.Location = New Point(ww, hh)
        hh += GroupBox39.Height + 10
        GroupBox38.Location = New Point(ww, hh)
        hh += GroupBox38.Height + 10
        Button3.Location = New Point(ww, hh)  'Save button
    End Sub
    'Diameter impeller is changed, recalculate and draw chart
    Private Sub Scale_rules_applied(ty As Integer, dia2 As Double, n2 As Double, ro2 As Double)
        Dim hh As Integer
        Dim Pow1, Q1, Pt1, Dia1, n1, Ro1 As Double

        'MessageBox.Show("ty= " & ty.ToString & " dia2= " & dia2.ToString & " n2= " & n2.ToString & " ro2= " & ro2.ToString)

        Try
            Dia1 = Tschets(ty).Tdata(0)     'waaier [mm]
            n1 = Tschets(ty).Tdata(1)       '[rpm]
            Ro1 = Tschets(ty).Tdata(2)      '[kg/m3]


            For hh = 0 To 11
                If Tschets(ty).TPtot(hh) > 0 Then           'Rest of the array is empty
                    Q1 = Tschets(ty).TFlow(hh)              '[Am3/s]
                    Pt1 = Tschets(ty).TPtot(hh)             '[Pa]
                    Pow1 = Tschets(ty).Tverm(hh)            '[kW]

                    If n1 < 1 Or Ro1 < 0.01 Or Dia1 < 0.01 Then  'Prevent devision bij zero
                        MessageBox.Show("Problem occured in line 1369")
                    End If

                    Tschets(ty).TFlow_scaled(hh) = Round(Scale_rule_cap(Q1, Dia1, dia2, n1, n2), 2)             '[m3/s]
                    Tschets(ty).TPtot_scaled(hh) = Round(Scale_rule_Pressure(Pt1, Dia1, dia2, n1, n2, Ro1, ro2), 0)
                    Tschets(ty).Tverm_scaled(hh) = Round(Scale_rule_Power(Pow1, Dia1, dia2, n1, n2, Ro1, ro2), 0)
                    Tschets(ty).Teff_scaled(hh) = Tschets(ty).Teff(hh)
                End If
            Next hh
            'MessageBox.Show("1380")
            draw_chart1(ty)     'This gives a problem
        Catch ex As Exception
            ' MessageBox.Show(ex.Message & " NumericUDown9_ValueChanged ")  ' Show the exception's message.
        End Try
    End Sub

    Private Sub do_Chart2()
        Dim schets_no As Integer

        'Clear all series And chart areas so we can re-add them
        Chart2.Series.Clear()
        Chart2.ChartAreas.Clear()
        Chart2.Titles.Clear()
        Chart2.ChartAreas.Add("ChartArea0")

        For schets_no = 0 To (ComboBox1.Items.Count - 1)
            Chart2.Series.Add("Series" & schets_no.ToString)
            Chart2.Series(schets_no).ChartArea = "ChartArea0"
            Chart2.Series(schets_no).ChartType = DataVisualization.Charting.SeriesChartType.Line
            Chart2.Series(schets_no).Name = (Tschets(schets_no).Tname)
            Chart2.Series(schets_no).BorderWidth = 1
        Next

        Chart2.Series(ComboBox2.SelectedIndex).BorderWidth = 4
        Chart2.Series(ComboBox2.SelectedIndex).Color = Color.Red

        Chart2.Titles.Add("Performance volgens de T-schetsen @ dia= 1000mm, ro= 1.20 kg/m3, 1480 rpm")
        Chart2.ChartAreas("ChartArea0").AxisX.Title = "Debiet [Am3/s]"
        Chart2.ChartAreas("ChartArea0").AxisY.Title = "Ptotaal [Pa]"

        For schets_no = 0 To (ComboBox1.Items.Count - 1)    'Fill line chart
            Scale_rules_applied(schets_no, 1000, 1480, 1.2) 'Compare alle fans on the same basis
            For hh = 0 To 11   'Fill line chart
                If Tschets(schets_no).TPtot(hh) > 0 Then           'Rest of the array is empty
                    Chart2.Series(schets_no).Points.AddXY(Tschets(schets_no).TFlow_scaled(hh), Tschets(schets_no).TPtot_scaled(hh))
                End If
            Next hh

            'MessageBox.Show("Tschets= " & Tschets(schets_no).Tname)
        Next schets_no
    End Sub
    Private Sub TabPage7_Paint(sender As Object, e As PaintEventArgs) Handles TabPage7.Paint
        do_Chart2()
    End Sub
    'Diameter impeller is changed, recalculate and draw chart
    Private Sub NumericUpDown9_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown9.ValueChanged
        Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
    End Sub
    Private Sub TabPage3_Enter(sender As Object, e As EventArgs) Handles TabPage3.Enter
        Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
    End Sub
    'Speed [rpm] is changed, recalculate and draw chart
    Private Sub NumericUpDown10_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown10.ValueChanged
        Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
    End Sub
    'Density [kg/Am3] is changed, recalculate and draw chart
    Private Sub NumericUpDown11_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown11.ValueChanged
        Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
    End Sub
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
    End Sub
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
    End Sub
    Private Sub NumericUpDown33_ValueChanged(sender As Object, e As EventArgs)
        Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
    End Sub
    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        Scale_rules_applied(ComboBox1.SelectedIndex, NumericUpDown9.Value, NumericUpDown10.Value, NumericUpDown11.Value)
    End Sub
    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged, NumericUpDown18.ValueChanged
        Dim qq, sigma02 As Double

        If (ComboBox3.SelectedIndex > -1) Then      'Prevent exceptions
            Dim words() As String = steel(ComboBox3.SelectedIndex).Split(";")
            TextBox33.Text = words(6)     'Density steel
            Label106.Text = words(20)     'Opmerkingen

            '--------------- select the strength @ temperature
            qq = NumericUpDown18.Value

            Select Case True
                Case (qq > -10 AndAlso qq <= 0)
                    Double.TryParse(words(9), sigma02)     'Sigma 0.2 [N/mm]
                Case (qq > 0 AndAlso qq <= 20)
                    Double.TryParse(words(10), sigma02)    'Sigma 0.2 [N/mm]
                Case (qq > 20 AndAlso qq <= 50)
                    Double.TryParse(words(11), sigma02)    'Sigma 0.2 [N/mm]
                Case (qq > 50 AndAlso qq <= 100)
                    Double.TryParse(words(12), sigma02)    'Sigma 0.2 [N/mm]
                Case (qq > 100 AndAlso qq <= 150)
                    Double.TryParse(words(13), sigma02)    'Sigma 0.2 [N/mm]
                Case (qq > 150 AndAlso qq <= 200)
                    Double.TryParse(words(13), sigma02)    'Sigma 0.2 [N/mm]
                Case (qq > 200 AndAlso qq <= 250)
                    Double.TryParse(words(14), sigma02)    'Sigma 0.2 [N/mm]
                Case (qq > 250 AndAlso qq <= 300)
                    Double.TryParse(words(15), sigma02)    'Sigma 0.2 [N/mm]
                Case (qq > 300 AndAlso qq <= 350)
                    Double.TryParse(words(16), sigma02)    'Sigma 0.2 [N/mm]
                Case (qq > 350 AndAlso qq <= 400)
                    Double.TryParse(words(17), sigma02)    'Sigma 0.2 [N/mm]
            End Select
            TextBox34.Text = sigma02.ToString
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click, NumericUpDown30.ValueChanged, NumericUpDown29.ValueChanged, NumericUpDown28.ValueChanged, NumericUpDown27.ValueChanged, NumericUpDown26.ValueChanged, NumericUpDown25.ValueChanged, NumericUpDown24.ValueChanged, NumericUpDown23.ValueChanged, NumericUpDown22.ValueChanged, TabPage5.Enter, NumericUpDown16.ValueChanged, ComboBox4.SelectedIndexChanged, RadioButton7.CheckedChanged, NumericUpDown15.ValueChanged

        Dim length_a, length_b, length_c, length_naaf As Double
        Dim dia_a, dia_b, dia_c, dia_naaf As Double
        Dim g_shaft_a, g_shaft_b, g_shaft_c, sg_staal As Double
        Dim J_shaft_a, J_shaft_b, J_shaft_c, J_naaf As Double
        Dim I_shaft_a, I_shaft_b, I_shaft_c As Double
        Dim gewicht_as, gewicht_waaier, gewicht_pulley, gewicht_naaf As Double
        Dim Force_combi1, Force_combi2, Force_combi3 As Double
        Dim N_kritisch_as As Double
        Dim N_max_doorbuiging As Double
        Dim Elasticiteitsm As Double
        Dim W_rpm As Double
        Dim F_onbalans, V_onbalans, hoeksnelheid As Double
        Dim F_a_hor, F_a_vert, F_a_combined As Double       'Bearing next impellar
        Dim F_b_hor, F_b_vert, F_b_combined As Double       'Bearing next coupling
        Dim F_axial As Double
        Dim dia_pulley, S_power, F_snaar, F_scheef As Double
        Dim Waaier_dia, Voorplaat_keel As Double

        W_rpm = NumericUpDown19.Value                       'Toerental waaier
        TextBox110.Text = NumericUpDown19.Value.ToString

        length_a = NumericUpDown22.Value / 1000         '[m]
        length_b = NumericUpDown23.Value / 1000         '[m]
        length_c = NumericUpDown24.Value / 1000         '[m]
        length_naaf = NumericUpDown29.Value / 1000      '[m]

        dia_a = NumericUpDown25.Value / 1000            '[m]
        dia_b = NumericUpDown26.Value / 1000            '[m]
        dia_c = NumericUpDown27.Value / 1000            '[m]
        dia_naaf = NumericUpDown28.Value / 1000         '[m]

        Double.TryParse(TextBox33.Text, sg_staal)
        g_shaft_a = PI / 4 * dia_a ^ 2 * length_a * sg_staal
        g_shaft_b = PI / 4 * dia_b ^ 2 * length_b * sg_staal
        g_shaft_c = PI / 4 * dia_c ^ 2 * length_c * sg_staal


        gewicht_as = g_shaft_a + g_shaft_b + g_shaft_c
        gewicht_waaier = NumericUpDown14.Value
        gewicht_pulley = NumericUpDown30.Value
        gewicht_naaf = PI / 4 * dia_naaf ^ 2 * length_naaf * sg_staal

        I_shaft_a = PI * dia_a ^ 4 / 64        'OppervlakTraagheid [m4]
        I_shaft_b = PI * dia_b ^ 4 / 64        'OppervlakTraagheid [m4]
        I_shaft_c = PI * dia_c ^ 4 / 64        'OppervlakTraagheid [m4]

        J_shaft_a = 0.5 * g_shaft_a * (dia_a / 2) ^ 2         'MassaTraagheid [kg.m2]
        J_shaft_b = 0.5 * g_shaft_b * (dia_b / 2) ^ 2         'MassaTraagheid [kg.m2]
        J_shaft_c = 0.5 * g_shaft_c * (dia_c / 2) ^ 2         'MassaTraagheid [kg.m2]
        J_naaf = 0.5 * gewicht_naaf * (dia_naaf / 2) ^ 2            'MassaTraagheid [kg.m2]

        '---------- Kritisch toerental formule 6.41 pagina 213--------------
        Elasticiteitsm = 210 * 1000 ^ 3                                 'in Pascal [N/m2]
        N_max_doorbuiging = ((length_a ^ 3 / I_shaft_a) + (length_a ^ 2 * length_b / I_shaft_b)) / (3 * Elasticiteitsm)
        N_max_doorbuiging *= (gewicht_waaier + gewicht_naaf + g_shaft_a) * 9.81
        N_kritisch_as = Sqrt(9.81 / N_max_doorbuiging) * 60 / (2 * PI)

        '--------- Kracht door onbalans----------
        V_onbalans = NumericUpDown15.Value / 1000          '[m/s]
        hoeksnelheid = (2 * PI * W_rpm) / 60
        F_onbalans = V_onbalans * hoeksnelheid * (gewicht_waaier + gewicht_naaf + g_shaft_a)

        '--------- Kracht door V_snaren----------
        If (ComboBox4.SelectedIndex > -1) Then      'Prevent exceptions
            Dim words() As String = emotor(ComboBox4.SelectedIndex).Split(";")
            S_power = words(0)      'Motor vermogen
            dia_pulley = NumericUpDown16.Value / 1000
            F_snaar = 975 * S_power * 20 / (W_rpm * dia_pulley * 0.5)
        End If

        '--------- Scheefstelling koppeling ---------------
        F_scheef = 5700 * Sqrt(S_power / W_rpm)

        '----------- Forces bearing vertical-------------
        Force_combi1 = (gewicht_waaier + gewicht_naaf) * 9.81 + F_onbalans
        Force_combi2 = gewicht_as * 9.81
        Force_combi3 = gewicht_pulley * 9.81

        F_a_vert = Abs(Force_combi1 * (length_a + length_b) + Force_combi2 * length_b * 0.5 - Force_combi3 * length_c) / length_b
        F_b_vert = Abs(Force_combi1 * length_a - Force_combi2 * length_b * 0.5 - Force_combi3 * (length_b + length_c)) / length_b

        '----------- Forces bearing horizontal-------------
        Force_combi1 = 00   ' 
        Force_combi2 = 00   ' 
        If RadioButton7.Checked Then    'direct drive
            Force_combi3 = F_scheef
            F_snaar = 0
        Else
            Force_combi3 = F_snaar
            F_scheef = 0
        End If
        F_a_hor = Abs(Force_combi1 * (length_a + length_b) + Force_combi2 * length_b * 0.5 - Force_combi3 * length_c) / length_b
        F_b_hor = Abs(Force_combi1 * length_a - Force_combi2 * length_b * 0.5 - Force_combi3 * (length_b + length_c)) / length_b

        '----------- Forces bearing combined-------------
        F_a_combined = Sqrt(F_a_vert ^ 2 + F_a_hor ^ 2)
        F_b_combined = Sqrt(F_b_vert ^ 2 + F_b_hor ^ 2)

        '--------Axial force Keel diameter------------
        If ComboBox1.SelectedIndex > -1 Then '------- schoepgewicht berekenen-----------
            Waaier_dia = NumericUpDown21.Value / 1000 '[m]
            Voorplaat_keel = Tschets(ComboBox1.SelectedIndex).Tdata(16) / 1000 * (Waaier_dia / 1.0)     '[m]
            F_axial = PI / 4 * Voorplaat_keel ^ 2 * NumericUpDown2.Value * 100
        End If
        ' MessageBox.Show("Voorplaat_keel= " & Voorplaat_keel.ToString & "  F_b_hor = " & F_b_hor.ToString)

        '----------- Present massa traagheid-------------
        TextBox35.Text = Round(J_shaft_a, 3).ToString   'Massa traagheid (0.5*M*R^2)
        TextBox39.Text = Round(J_shaft_b, 2).ToString   'Massa traagheid (0.5*M*R^2)
        TextBox41.Text = Round(J_shaft_c, 3).ToString   'Massa traagheid (0.5*M*R^2)
        TextBox92.Text = Round(J_naaf, 3).ToString      'Massa traagheid (0.5*M*R^2)

        '----------- Present gewicht------------------
        TextBox46.Text = Round(g_shaft_a, 1).ToString
        TextBox48.Text = Round(g_shaft_b, 1).ToString
        TextBox52.Text = Round(g_shaft_c, 1).ToString
        TextBox93.Text = Round(gewicht_naaf, 1).ToString

        TextBox102.Text = Round(g_shaft_a + g_shaft_b + g_shaft_c + gewicht_naaf + gewicht_pulley, 1).ToString 'Totaal gewicht impellar
        TextBox47.Text = Round(N_kritisch_as, 0).ToString   '[RPM]

        '--------------- krachten---------------
        TextBox97.Text = Round(F_onbalans, 0).ToString      'Force inbalans
        TextBox98.Text = Round(F_snaar, 0).ToString         'Force trekkracht snaar
        TextBox99.Text = Round(F_scheef, 0).ToString        'Force scheefstelling (geen snaar)
        TextBox100.Text = Round(F_a_combined, 0).ToString   'Force lager A hor+vert combined
        TextBox101.Text = Round(F_b_combined, 0).ToString   'Force lager B hor+vert combined
        TextBox19.Text = Round(F_axial, 0).ToString         'Force axial

        TextBox111.Text = Round(N_max_doorbuiging * 1000, 3).ToString      'Max doorbuiging in [mm]
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click, NumericUpDown8.ValueChanged, NumericUpDown7.ValueChanged, NumericUpDown6.ValueChanged, NumericUpDown5.ValueChanged, NumericUpDown4.ValueChanged, NumericUpDown3.ValueChanged, NumericUpDown2.ValueChanged, NumericUpDown13.ValueChanged, NumericUpDown12.ValueChanged, NumericUpDown1.ValueChanged, ComboBox1.SelectedIndexChanged, RadioButton4.CheckedChanged, RadioButton3.CheckedChanged, CheckBox4.CheckedChanged, CheckBox5.CheckedChanged, NumericUpDown33.ValueChanged, ComboBox7.SelectedIndexChanged, TabPage1.Enter
        If TabControl1.SelectedTab.Name = "TabPage1" Then
            ComboBox7.SelectedIndex = ComboBox1.SelectedIndex
        End If
        Selectie_1()
    End Sub
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click, TabPage4.Enter
        Dim spez_drehz, p_tot, Act_flow As Double
        Dim Ltot, Lp63, Lp125, Lp250, Lp500, Lp1000, Lp2000, Lp4000, LP8000 As Double


        Double.TryParse(TextBox123.Text, spez_drehz)
        p_tot = NumericUpDown2.Value * 100                              '[Pa]
        Double.TryParse(TextBox22.Text, Act_flow)                       '[m3/hr]        
        Label152.Text = "Waaier type" & Tschets(ComboBox1.SelectedIndex).Tname

        'Lws = 2.9513409 * Log(spez_drehz) + 26.0752394                  'Willi Bohl pagina 45 and 51 for check
        'Ltot = Lws + 10 * Log10(Act_flow / 3600) + 20 * Log10(p_tot)    ' +/- 5 db
        Ltot = 27 + 10 * Log10(Act_flow / 3600) + 20 * Log10(p_tot)     ' http://www.schweizer-fn.de/lueftung/ventilator/ventilator.php

        Lp63 = Ltot - 9
        Lp125 = Ltot - 8
        Lp250 = Ltot - 7
        Lp500 = Ltot - 12
        Lp1000 = Ltot - 17
        Lp2000 = Ltot - 22
        Lp4000 = Ltot - 26
        LP8000 = Ltot - 31

        '---------------- input data--------------------------
        TextBox112.Text = Round(Act_flow, 0).ToString       'Debiet [m3/hr]
        TextBox127.Text = Round(Act_flow / 3600, 2).ToString 'Debiet [m3/s]
        TextBox113.Text = Round(p_tot / 100, 0).ToString    'Dp total [mbar]
        TextBox126.Text = Round(p_tot, 0).ToString          'Dp total [Pa]
        TextBox114.Text = Round(spez_drehz, 0).ToString     'Spez. drehzahl

        '---------- results----------------------------
        TextBox125.Text = Round(Ltot, 0).ToString

        '----------opgesplits in banden--------------
        TextBox115.Text = Round(Lp63, 0).ToString       '
        TextBox116.Text = Round(Lp125, 0).ToString
        TextBox117.Text = Round(Lp250, 0).ToString
        TextBox118.Text = Round(Lp500, 0).ToString
        TextBox119.Text = Round(Lp1000, 0).ToString
        TextBox120.Text = Round(Lp2000, 0).ToString
        TextBox121.Text = Round(Lp4000, 0).ToString
        TextBox122.Text = Round(LP8000, 0).ToString

    End Sub
    'Calculate compressor
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click, NumericUpDown43.ValueChanged, NumericUpDown42.ValueChanged, NumericUpDown39.ValueChanged, NumericUpDown37.ValueChanged, NumericUpDown36.ValueChanged, NumericUpDown35.ValueChanged, NumericUpDown34.ValueChanged, ComboBox5.SelectedIndexChanged, RadioButton6.CheckedChanged, RadioButton5.CheckedChanged, NumericUpDown48.ValueChanged, NumericUpDown47.ValueChanged, NumericUpDown45.ValueChanged, NumericUpDown44.ValueChanged, NumericUpDown49.ValueChanged
        Scale_rules_compressor()
    End Sub
    Private Sub Scale_rules_compressor()
        Dim ty As Integer
        Dim Pow0, Q0, dia0, n0 As Double                                                'From Tschets
        Dim Ptot0, ro0_inlet, temp0 As Double                                           'Initial conditions
        Dim Volume_debiet As Double                                                     '[m3/hr] is identiek per waaier (onafhankelijk van de density)
        Dim massa_debiet As Double
        Dim Pow1, P1_in, P1_out, dia1, n1, ro_fan1_inlet, ro_fan1_outlet, temp1 As Double       'Impellar #1 
        Dim Pow2, P2_in, P2_out, dia2, n2, ro_fan2_inlet, ro_fan2_outlet, temp2 As Double       'Impellar #2 
        Dim Pow3, P3_in, P3_out, dia3, n3, ro_fan3_inlet, ro_fan3_outlet, temp3 As Double       'Impellar #3 
        Dim dia_omloop1, dp_omloop_1, phi_1, velos_1, area_1 As Double                          'Omloop #1
        Dim dia_omloop2, dp_omloop_2, phi_2, velos_2, area_2 As Double                          'Omloop #2
        Dim Power_total As Double

        Try
            If ty > -1 Then
                ty = ComboBox5.SelectedIndex
                '---------- data Tschetsen-----------------
                dia0 = Tschets(ty).Tdata(0)         'waaier [mm]
                n0 = Tschets(ty).Tdata(1)           '[rpm]
                ro0_inlet = Tschets(ty).Tdata(2)    '[kg/m3]
                Ptot0 = Tschets(ty).werkp_opT(1)    'P_totaal [Pa]
                Pow0 = Tschets(ty).werkp_opT(3)     '[kW]
                Q0 = Tschets(ty).werkp_opT(4)       '[Am3/s]

                '---------- data gewenst---------------------
                temp0 = NumericUpDown43.Value       '[c]
                dia1 = NumericUpDown36.Value
                n1 = NumericUpDown35.Value
                ro_fan1_inlet = NumericUpDown34.Value
                P1_in = NumericUpDown49.Value * 100                          'Inlet pressure [Pa Gauge]

                '---------- omloop---------------------------
                phi_1 = NumericUpDown37.Value                   '[-]
                phi_2 = NumericUpDown42.Value                   '[-]
                dia_omloop1 = NumericUpDown47.Value / 1000      '[m]
                dia_omloop2 = NumericUpDown48.Value / 1000      '[m]


                velos_1 = NumericUpDown44.Value                 '[m/s]
                velos_2 = NumericUpDown45.Value                 '[m/s]

                '--------Impellar data-----
                dia2 = dia1     'Impellar #1 and #2 have same diameter
                dia3 = dia1     'Impellar #1 and #3 have same diameter
                n2 = n1         'Impellar #1 and #2 have same speed
                n3 = n1         'Impellar #1 and #3 have same speed

                '=============================================== PROBLEM PSTAT and P_total seem to be mixed up=================
                '================================= CHECK CHECK=========================================

                Volume_debiet = Round(Scale_rule_cap(Q0, dia0, dia1, n0, n1), 2)                        '[Am3/s]
                massa_debiet = Volume_debiet * ro_fan1_inlet                                            '[kg/s= m3/s* kg/m3]

                '-------------------------- Waaier #1 ----------------------
                P1_out = P1_in + Round(Scale_rule_Pressure(Ptot0, dia0, dia1, n0, n1, ro0_inlet, ro_fan1_inlet), 0)    '[Pa]
                Pow1 = Round(Scale_rule_Power(Pow0, dia0, dia1, n0, n1, ro0_inlet, ro_fan1_inlet), 0)               '[kW] 
                temp1 = temp0 + (Pow1 / (cp_air * massa_debiet))                                                    '[c]

                '----------- present------------------------
                TextBox136.Text = Round(P1_in / 100, 0).ToString                            '[mbarg] inlet (atmospheric)
                TextBox12.Text = Round(P1_out / 100, 0).ToString                            '[mbar] outlet
                TextBox129.Text = Round(temp1, 0).ToString                                  '[c]
                TextBox132.Text = Round(Pow1, 0).ToString                                   '[kW]
                TextBox135.Text = Round(massa_debiet * 3600, 0).ToString                    '[kg/hr]
                TextBox140.Text = Round(Volume_debiet * 3600, 0).ToString                   '[m3/hr]

                '------------------- pressure loss omloop #1 -----------------------------
                '--------------------- dp = phi* 1/2 ro*v^2 --------------
                '------------------- calc ro2 @ omloop inlet ----------------
                ro_fan1_outlet = calc_density(G_density_act_zuig, (Ptot0 + 101300), (P1_out + 101300), temp0, temp1)
                NumericUpDown38.Value = Round(ro_fan1_outlet, 2).ToString       'Density outlet

                '----------------- diameter omloop #1 --------------------
                area_1 = (massa_debiet / ro_fan1_outlet) / velos_1              '[m2]
                dia_omloop1 = Sqrt(area_1 / 4 * PI) * 1000                      '[mm]
                NumericUpDown47.Value = Round(dia_omloop1, 0).ToString          '[mm]

                dp_omloop_1 = 0.5 * phi_1 * velos_1 ^ 2 * ro_fan1_outlet        '[Pa]
                TextBox64.Text = Round(dp_omloop_1 / 100, 0).ToString           '[mbar]
                P2_in = P1_out - dp_omloop_1                                    '[Pa]

                ro_fan2_inlet = calc_density(G_density_act_zuig, (Ptot0 + 101300), (P2_in + 101300), temp0, temp1)

                NumericUpDown39.Value = Round(ro_fan2_inlet, 2).ToString        '[kg/m3] Density inlet

                '-------------------------- Waaier #2 ----------------------
                '---------------------------------------------------------------
                P2_out = P2_in + Round(Scale_rule_Pressure(Ptot0, dia0, dia2, n0, n2, ro0_inlet, ro_fan2_inlet), 0)    '[Pa]
                Pow2 = Round(Scale_rule_Power(Pow0, dia0, dia2, n0, n2, ro0_inlet, ro_fan2_inlet), 0)               '[kW] 
                temp2 = temp0 + ((Pow1 + Pow2) / (cp_air * massa_debiet))                                           '[c]

                '----------- present------------------------
                TextBox137.Text = Round(P2_in / 100, 0).ToString                '[mbar]
                TextBox130.Text = Round(temp2, 0).ToString                      '[c]
                TextBox80.Text = Round(P2_out / 100, 0).ToString                '[mbar] outlet
                TextBox133.Text = Round(Pow2, 0).ToString                       '[kW]

                '------------------- pressure loss omloop #2 -----------------------------
                '--------------------- dp = phi* 1/2 ro*v^2 --------------
                '------------------- calc ro2 @ omloop inlet ----------------
                ro_fan2_outlet = calc_density(G_density_act_zuig, (Ptot0 + 101300), (P2_out + 101300), temp0, temp2)
                NumericUpDown40.Value = Round(ro_fan2_outlet, 2).ToString       'Density outlet


                '----------------- diameter omloop #2 --------------------
                area_2 = (massa_debiet / ro_fan2_outlet) / velos_2              '[m2]
                dia_omloop2 = Sqrt(area_2 / 4 * PI) * 1000                      '[mm]
                NumericUpDown48.Value = Round(dia_omloop2, 0).ToString          '[mm]

                dp_omloop_2 = 0.5 * phi_2 * velos_2 ^ 2 * ro_fan2_outlet        '[Pa]
                TextBox128.Text = Round(dp_omloop_2 / 100, 0).ToString          '[mbar]

                P3_in = P2_out - dp_omloop_2                                    '[Pa]

                ro_fan3_inlet = calc_density(G_density_act_zuig, (Ptot0 + 101300), (P3_in + 101300), temp0, temp2)
                NumericUpDown41.Value = Round(ro_fan3_inlet, 2).ToString        '[kg/m3] Density inlet

                '-------------------------- Waaier #3 ----------------------
                '---------------------------------------------------------------
                P3_out = P3_in + Round(Scale_rule_Pressure(Ptot0, dia0, dia3, n0, n3, ro0_inlet, ro_fan3_inlet), 0) '[Pa]
                Pow3 = Round(Scale_rule_Power(Pow0, dia0, dia3, n0, n3, ro0_inlet, ro_fan3_inlet), 0)   '[kW] 
                temp3 = temp0 + ((Pow1 + Pow2 + Pow3) / (cp_air * massa_debiet))

                ro_fan3_outlet = calc_density(G_density_act_zuig, (Ptot0 + 101300), (P3_out + 101300), temp0, temp3)
                NumericUpDown46.Value = Round(ro_fan3_outlet, 2).ToString       'Density outlet

                '-------------------------- Aantal trappen ----------------------
                If RadioButton5.Checked Then    '2 Traps
                    GroupBox28.Visible = False
                    GroupBox29.Visible = False
                    TextBox80.BackColor = Color.FromArgb(192, 255, 192)
                    TextBox130.BackColor = Color.FromArgb(192, 255, 192)
                    TextBox133.BackColor = Color.FromArgb(192, 255, 192)
                    Power_total = Pow1 + Pow2                                           '[kW]
                    TextBox147.Text = Round((P2_out - P1_in) / 100, 0).ToString         '[mbar] dP fan
                Else
                    TextBox80.BackColor = SystemColors.Window
                    TextBox130.BackColor = SystemColors.Window
                    TextBox133.BackColor = SystemColors.Window
                    GroupBox28.Visible = True
                    GroupBox29.Visible = True
                    Power_total = Pow1 + Pow2 + Pow3                                    '[kW]
                    TextBox147.Text = Round((P3_out - P1_in) / 100, 0).ToString         '[mbar] dP fan
                End If

                '----------- present------------------------
                TextBox138.Text = Round(P3_in / 100, 0).ToString                    '[mbar]
                TextBox131.Text = Round(temp3, 0).ToString                          '[c]
                TextBox82.Text = Round(P3_out / 100, 0).ToString                    '[mbar] outlet
                TextBox134.Text = Round(Pow3, 0).ToString                           '[kW]
                TextBox139.Text = Round(Power_total, 0).ToString                    '[kW]

            End If
        Catch ex As Exception
            'MessageBox.Show(ex.Message & " Error 4316 ")  ' Show the exception's message.
        End Try
    End Sub
    'Calculate density
    '1= inlet, 2= outlet
    'Temperatures in celsius
    'Pressure in Pascal absolute
    Private Function calc_density(density1 As Double, pressure1 As Double, pressure2 As Double, temperature1 As Double, temperature2 As Double)
        Dim density2 As Double
        temperature1 += 273.15
        temperature2 += 273.15

        If (pressure1 < 90000) Or (pressure1 < 90000) Then
            MessageBox.Show("Density calculation warning Pressure must be in Pa absolute")
        End If

        density2 = density1 * (pressure2 / pressure1) * (temperature1 / temperature2)
        Return (density2)
    End Function
    'Calculate the labyrinth loss
    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click, NumericUpDown55.ValueChanged, NumericUpDown54.ValueChanged, NumericUpDown53.ValueChanged, NumericUpDown52.ValueChanged, NumericUpDown51.ValueChanged, NumericUpDown50.ValueChanged, TabPage9.Enter
        Dim as_diam, spalt_breed, spalt_opp, rho, dpres, spalt_velos, no_rings, spalt_loss, contractie As Double

        as_diam = NumericUpDown51.Value                 '[mm]
        spalt_breed = NumericUpDown50.Value             '[mm]
        rho = NumericUpDown53.Value                     '[kg/m3]
        no_rings = NumericUpDown55.Value                '[-]
        contractie = NumericUpDown54.Value              '[-]
        dpres = NumericUpDown52.Value * 100 / no_rings  '[Pa] pressure loss per ring

        spalt_opp = PI / 4 * as_diam * spalt_breed          '[mm2]

        'Principle pressure is transferred into speed
        spalt_velos = contractie * Sqrt(dpres * 2 / rho)

        spalt_loss = spalt_velos * spalt_opp / 1000 ^ 2 * 36000 * rho   '[kg/hr]

        TextBox143.Text = Round(spalt_opp, 0).ToString      '[mm2]
        TextBox144.Text = Round(spalt_velos, 1).ToString    '[m/s]
        TextBox145.Text = Round(spalt_loss, 1).ToString    '[kg/hr]
    End Sub


End Class