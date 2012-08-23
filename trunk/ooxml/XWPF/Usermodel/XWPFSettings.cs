/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */
namespace NPOI.XWPF.UserModel
{
    using System;
    using NPOI.OpenXmlFormats.Wordprocessing;
    using NPOI.OpenXml4Net.OPC;
    using System.IO;
using System.Xml.Serialization;
    using System.Xml;

    public class XWPFSettings : POIXMLDocumentPart
    {

        private CT_Settings ctSettings;

        public XWPFSettings(PackagePart part, PackageRelationship rel)
            : base(part, rel)
        {

        }

        public XWPFSettings()
            : base()
        {
            ctSettings = new CT_Settings();
        }


        internal override void OnDocumentRead()
        {
            base.OnDocumentRead();
            ReadFrom(GetPackagePart().GetInputStream());
        }

        /**
         * Set zoom.<br/>
         * In the zoom tag inside Settings.xml file <br/>
         * it Sets the value of zoom
         * <br/>
         * sample snippet from Settings.xml
         * <pre>
         *    &lt;w:zoom w:percent="50" /&gt;
         * <pre>
         * @return percentage as an integer of zoom level
         */
        public long GetZoomPercent()
        {
            CT_Zoom zoom = ctSettings.zoom;
            //if (!ctSettings.IsSetZoom())
            //{
            //    zoom = ctSettings.AddNewZoom();
            //}
            //else
            //{
            //    zoom = ctSettings.zoom;
            //}

            return long.Parse(zoom.percent);
        }

        /**
         * Set zoom.<br/>
         * In the zoom tag inside Settings.xml file <br/>
         * it Sets the value of zoom
         * <br/>
         * sample snippet from Settings.xml 
         * <pre>
         *    &lt;w:zoom w:percent="50" /&gt; 
         * <pre>
         */
        public void SetZoomPercent(long zoomPercent)
        {
            //if (!ctSettings.IsSetZoom())
            //{
            //    ctSettings.AddNewZoom();
            //}
            CT_Zoom zoom = ctSettings.zoom;
            //zoom.Percent = (BigInt32.ValueOf(zoomPercent));
            zoom.percent = zoomPercent.ToString();
        }

        /**
         * Verifies the documentProtection tag inside Settings.xml file <br/>
         * if the protection is enforced (w:enforcement="1") <br/>
         * and if the kind of protection Equals to passed (STDocProtect.Enum editValue) <br/>
         * 
         * <br/>
         * sample snippet from Settings.xml
         * <pre>
         *     &lt;w:settings  ... &gt;
         *         &lt;w:documentProtection w:edit=&quot;readOnly&quot; w:enforcement=&quot;1&quot;/&gt;
         * </pre>
         * 
         * @return true if documentProtection is enforced with option ReadOnly
         */
        public bool IsEnforcedWith(ST_DocProtect editValue) {
            CT_DocProtect ctDocProtect = ctSettings.documentProtection;

            if (ctDocProtect == null)
            {
                return false;
            }

            return ctDocProtect.enforcement.Equals(ST_OnOff.Value1) && ctDocProtect.edit.Equals(editValue);
        }

        /**
         * Enforces the protection with the option specified by passed editValue.<br/>
         * <br/>
         * In the documentProtection tag inside Settings.xml file <br/>
         * it Sets the value of enforcement to "1" (w:enforcement="1") <br/>
         * and the value of edit to the passed editValue (w:edit="[passed editValue]")<br/>
         * <br/>
         * sample snippet from Settings.xml
         * <pre>
         *     &lt;w:settings  ... &gt;
         *         &lt;w:documentProtection w:edit=&quot;[passed editValue]&quot; w:enforcement=&quot;1&quot;/&gt;
         * </pre>
         */
        public void SetEnforcementEditValue(ST_DocProtect editValue) {
            safeGetDocumentProtection().enforcement=(ST_OnOff.Value1);
            safeGetDocumentProtection().edit=(editValue);
        }

        /**
         * Removes protection enforcement.<br/>
         * In the documentProtection tag inside Settings.xml file <br/>
         * it Sets the value of enforcement to "0" (w:enforcement="0") <br/>
         */
        public void RemoveEnforcement()
        {
            safeGetDocumentProtection().enforcement = (ST_OnOff.Value0);
        }


        protected override void Commit()
        {

            /*XmlOptions xmlOptions = new XmlOptions(DEFAULT_XML_OPTIONS);
            xmlOptions.SaveSyntheticDocumentElement=(new QName(CTSettings.type.Name.NamespaceURI, "settings"));
            Dictionary<String, String> map = new Dictionary<String, String>();
            map.Put("http://schemas.openxmlformats.org/wordprocessingml/2006/main", "w");
            xmlOptions.SaveSuggestedPrefixes=(map);*/
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] {
                new XmlQualifiedName("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main")});
            PackagePart part = GetPackagePart();
            Stream out1 = part.GetOutputStream();
            SettingsDocument sd = new SettingsDocument(ctSettings);
            sd.Save(out1, namespaces);
            out1.Close();
        }

        private CT_DocProtect safeGetDocumentProtection()
        {
            CT_DocProtect documentProtection = ctSettings.documentProtection;
            if (documentProtection == null)
            {
                documentProtection = new CT_DocProtect();
                ctSettings.documentProtection = (documentProtection);
            }
            return ctSettings.documentProtection;
        }

        private void ReadFrom(Stream inputStream)
        {
            try
            {
                ctSettings = SettingsDocument.Parse(inputStream).Settings;
            }
            catch (Exception e)
            {
                throw new Exception("SettingsDocument parse failed", e);
            }
        }

    }

}