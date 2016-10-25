using System;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;

namespace LYH.WorkOrder.share
    {
        public class WordClass : IDisposable
        {
            #region �ֶ�
            private _Application _mWordApp = null;
            private _Document _mDocument = null;
            private object _missing = System.Reflection.Missing.Value;
            #endregion

            #region ���캯������������
            public WordClass()
            {
                _mWordApp = new Application();
            }
            ~WordClass()
            {
                try
                {
                    if (_mWordApp != null)
                        _mWordApp.Quit(ref _missing, ref _missing, ref _missing);
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
            }
            #endregion

            #region ����
            public _Document Document => _mDocument;
            public _Application WordApplication => _mWordApp;

            public int WordCount
            {
                get
                {
                    if (_mDocument != null)
                    {
                        var rng = _mDocument.Content;
                        rng.Select();
                        return _mDocument.Characters.Count;
                    }
                    else
                        return -1;
                }
            }
            public object Missing => _missing;

            #endregion

            #region ��������
            #region CreateDocument
            public void CreateDocument(string template)
            {
                object objTemplate = template;
                if (template.Length <= 0) objTemplate = _missing;
                _mDocument = _mWordApp.Documents.Add(ref objTemplate, ref _missing, ref _missing, ref _missing);
            }
            public void CreateDocument()
            {
                CreateDocument("");
            }
            #endregion
            #region OpenDocument
            public void OpenDocument(string fileName, bool readOnly)
            {
                object objFileName = fileName;
                object objReadOnly = readOnly;
                _mDocument = _mWordApp.Documents.Open(ref objFileName, ref _missing, ref objReadOnly, ref _missing,
                    ref _missing, ref _missing, ref _missing, ref _missing, ref _missing, ref _missing, ref _missing, ref _missing,
                    ref _missing, ref _missing, ref _missing, ref _missing);
            }
            public void OpenDocument(string fileName)
            {
                OpenDocument(fileName, false);
            }
            #endregion
            #region Save & SaveAs
            public void Save()
            {
                if (_mDocument != null)
                    _mDocument.Save();
            }
            public void SaveAs(string fileName)
            {
                object objFileName = fileName;
                if (_mDocument != null)
                {
                    _mDocument.SaveAs(ref objFileName, ref _missing, ref _missing, ref _missing, ref _missing,
                        ref _missing, ref _missing, ref _missing, ref _missing, ref _missing, ref _missing, ref _missing,
                        ref _missing, ref _missing, ref _missing, ref _missing);
                }
            }
            #endregion
            #region Close
            public void Close(bool isSaveChanges)
            {
                object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
                if (isSaveChanges)
                    saveChanges = WdSaveOptions.wdSaveChanges;
                if (_mDocument != null)
                    _mDocument.Close(ref saveChanges, ref _missing, ref _missing);
            }
            #endregion
            #region �������
            /// <summary>
            /// ���ͼƬ
            /// </summary>
            /// <param name="picName"></param>
            public void AddPicture(string picName)
            {
                if (_mWordApp != null)
                    _mWordApp.Selection.InlineShapes.AddPicture(picName, ref _missing, ref _missing, ref _missing);
            }
            /// <summary>
            /// ����ҳü
            /// </summary>
            /// <param name="text"></param>
            /// <param name="align"></param>
            public void SetHeader(string text, WdParagraphAlignment align)
            {
                _mWordApp.ActiveWindow.View.Type = WdViewType.wdOutlineView;
                _mWordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekPrimaryHeader;
                _mWordApp.ActiveWindow.ActivePane.Selection.InsertAfter(text); //�����ı�
                _mWordApp.Selection.ParagraphFormat.Alignment = align;  //���ö��뷽ʽ
                _mWordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument; // ����ҳü����
            }
            /// <summary>
            /// ����ҳ��
            /// </summary>
            /// <param name="text"></param>
            /// <param name="align"></param>
            public void SetFooter(string text, WdParagraphAlignment align)
            {
                _mWordApp.ActiveWindow.View.Type = WdViewType.wdOutlineView;
                _mWordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekPrimaryFooter;
                _mWordApp.ActiveWindow.ActivePane.Selection.InsertAfter(text); //�����ı�
                _mWordApp.Selection.ParagraphFormat.Alignment = align;  //���ö��뷽ʽ
                _mWordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument; // ����ҳü����
            }
            #endregion
            #region Print

public void PrintOut()
            {
                object copies = "1";
                object pages = "";
                object range = WdPrintOutRange.wdPrintAllDocument;
                object items = WdPrintOutItem.wdPrintDocumentContent;
                object pageType = WdPrintOutPages.wdPrintAllPages;
                object oTrue = true;
                object oFalse = false;
                _mDocument.PrintOut(
                    ref oTrue, ref oFalse, ref range, ref _missing, ref _missing, ref _missing,
                    ref items, ref copies, ref pages, ref pageType, ref oFalse, ref oTrue,
                    ref _missing, ref oFalse, ref _missing, ref _missing, ref _missing, ref _missing);
            }
            public void PrintPreview()
            {
                if (_mDocument != null)
                    _mDocument.PrintPreview();
            }
            #endregion
            public void Paste()
            {
                try
                {
                    if (_mDocument != null)
                    {
                        _mDocument.ActiveWindow.Selection.Paste();
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                }
            }
            #endregion

            #region �ĵ��е��ı��Ͷ���
            public void AppendText(string text)
            {
                var currentSelection = _mWordApp.Selection;
                // Store the user's current Overtype selection
                var userOvertype = _mWordApp.Options.Overtype;
                // Make sure Overtype is turned off.
                if (_mWordApp.Options.Overtype)
                {
                    _mWordApp.Options.Overtype = false;
                }
                // Test to see if selection is an insertion point.
                if (currentSelection.Type == WdSelectionType.wdSelectionIP)
                {
                    currentSelection.TypeText(text);
                    currentSelection.TypeParagraph();
                }
                else
                    if (currentSelection.Type == WdSelectionType.wdSelectionNormal)
                    {
                        // Move to start of selection.
                        if (_mWordApp.Options.ReplaceSelection)
                        {
                            object direction = WdCollapseDirection.wdCollapseStart;
                            currentSelection.Collapse(ref direction);
                        }
                        currentSelection.TypeText(text);
                        currentSelection.TypeParagraph();
                    }
                // Restore the user's Overtype selection
                _mWordApp.Options.Overtype = userOvertype;
            }
            #endregion

            #region �������滻�ĵ��е��ı�
            public void Replace(string oriText, string replaceText)
            {
                object replaceAll = WdReplace.wdReplaceAll;
                _mWordApp.Selection.Find.ClearFormatting();
                _mWordApp.Selection.Find.Text = oriText;
                _mWordApp.Selection.Find.Replacement.ClearFormatting();
                _mWordApp.Selection.Find.Replacement.Text = replaceText;
                _mWordApp.Selection.Find.Execute(
                    ref _missing, ref _missing, ref _missing, ref _missing, ref _missing,
                    ref _missing, ref _missing, ref _missing, ref _missing, ref _missing,
                    ref replaceAll, ref _missing, ref _missing, ref _missing, ref _missing);
            }

            #endregion

            #region �ĵ��е�Word���
            /// <summary>
            /// ���ĵ��в�����
            /// </summary>
            /// <param name="startIndex">��ʼλ��</param>
            /// <param name="endIndex">����λ��</param>
            /// <param name="rowCount">����</param>
            /// <param name="columnCount">����</param>
            /// <returns></returns>
            public Table AppendTable(int startIndex, int endIndex, int rowCount, int columnCount)
            {
                object start = startIndex;
                object end = endIndex;
                var tableLocation = _mDocument.Range(ref start, ref end);
                return _mDocument.Tables.Add(tableLocation, rowCount, columnCount, ref _missing, ref _missing);
            }
            /// <summary>
            /// �����
            /// </summary>
            /// <param name="table"></param>
            /// <returns></returns>
            public Row AppendRow(Table table)
            {
                object row = table.Rows[1];
                return table.Rows.Add(ref row);
            }
            /// <summary>
            /// �����
            /// </summary>
            /// <param name="table"></param>
            /// <returns></returns>
            public Column AppendColumn(Table table)
            {
                object column = table.Columns[1];
                return table.Columns.Add(ref column);
            }
            /// <summary>
            /// ���õ�Ԫ����ı��Ͷ��뷽ʽ
            /// </summary>
            /// <param name="cell">��Ԫ��</param>
            /// <param name="text">�ı�</param>
            /// <param name="align">���뷽ʽ</param>
            public void SetCellText(Cell cell, string text, WdParagraphAlignment align)
            {
                cell.Range.Text = text;
                cell.Range.ParagraphFormat.Alignment = align;
            }
            #endregion

            #region IDisposable ��Ա
            public void Dispose()
            {
                try
                {
                    if (_mWordApp != null)
                        _mWordApp.Quit(ref _missing, ref _missing, ref _missing);
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
            }
            #endregion
        }
    }


