﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.Enums;
using ListenManager.Interfaces;
using ListenManager.Managers.Messages;
using ListenManager.Managers.Util;

namespace ListenManager.Managers
{
    public class ListViewManager : BaseManager
    {
        private readonly VerzeichnisHandler _handler;
        private readonly IWindowService _windowService;
        private readonly Visibilities _defaultVisibilities;

        private Visibilities _fieldVisibility;
        private MitgliedsListe _selectedListe;
        private VereinsMitglied _selectedMitglied;
        private ObservableCollection<VereinsMitglied> _mitglieder;
        private ObservableCollection<MitgliedsListe> _listen;

        private ICommand _updateMemberCommand;
        private ICommand _editListCommand;
        private ICommand _addListCommand;

        public ListViewManager()
        {
            _handler = VerzeichnisHandler.Instance;
            _windowService = new WindowService();

            _defaultVisibilities = _handler.DefaultVisibilities;

            Listen = _handler.GetAllVerzeichnisse();
            SelectedListe = Listen.FirstOrDefault();

            if (SelectedListe == null) { return; }
            VisibleFields = _handler.GetFieldVisibilitiesForGivenList(SelectedListe.SourceVerzeichnis.ID);

            Messenger.Default.Register<UpdateListMessage>(this, HandleUpdateMessage);
        }

        public Visibilities VisibleFields
        {
            get => _fieldVisibility;
            set
            {
                if(value == null) return;
                if(value.Equals(_fieldVisibility)) return;
                _fieldVisibility = value;
                OnPropertyChanged(nameof(VisibleFields));
            }
        }

        public ObservableCollection<MitgliedsListe> Listen
        {
            get => _listen;
            set
            {
                if(value == null) return;
                if(value.Equals(_listen)) return;
                _listen = value;
                OnPropertyChanged(nameof(Listen));
            }
        }

        public ObservableCollection<VereinsMitglied> Mitglieder
        {
            get => _mitglieder;
            set
            {
                if(value == null) return;
                if(value.Equals(_mitglieder)) return;
                _mitglieder = value;
                OnPropertyChanged(nameof(Mitglieder));
            }
        }

        public MitgliedsListe SelectedListe
        {
            get => _selectedListe;
            set
            {
                if(value == null) return;
                if(value.Equals(_selectedListe)) return;
                _selectedListe = value;
                UpdateListe();
                OnPropertyChanged(nameof(SelectedListe));
            }
        }

        public VereinsMitglied SelectedMitglied
        {
            get => _selectedMitglied;
            set
            {
                if(value == null) return;
                if(value.Equals(_selectedMitglied)) return;
                _selectedMitglied = value;
                OnPropertyChanged(nameof(SelectedMitglied));
            }
        }

        public ICommand UpdateMemberCommand => _updateMemberCommand ?? (_updateMemberCommand = new RelayCommand(UpdateMember));
        public ICommand EditListCommand => _editListCommand ?? (_editListCommand = new RelayCommand(ShowEditListDialog));
        public ICommand AddListCommand => _addListCommand ?? (_addListCommand = new RelayCommand(ShowAddListDialog));

        private void UpdateMember()
        {
            var shared = SharedProperties.Instance;
            shared.IsEditMemberDialog = true;
            shared.MemberToEdit = SelectedMitglied;
            _windowService.ShowDialog("EditMemberPage");
        }

        private void UpdateListe ()
        {
            if (SelectedListe.Type != ListType.UserCreated)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (SelectedListe.Type)
                {
                    case ListType.Jugend:
                        Mitglieder = _handler.GetJugend();
                        break;
                    case ListType.Erwachsene:
                        Mitglieder = _handler.GetErwachsene();
                        break;
                }

                VisibleFields = _defaultVisibilities;
            }
            else
            {
                Mitglieder = _handler.GetMitgliederInVerzeichnis(SelectedListe.SourceVerzeichnis.ID);
                VisibleFields = _handler.GetFieldVisibilitiesForGivenList(SelectedListe.SourceVerzeichnis.ID);
            }
            
        }

        private void ShowEditListDialog()
        {
            if (SelectedListe.Type != ListType.UserCreated) return;

            var shared = SharedProperties.Instance;
            shared.IsEditListDialog = true;
            shared.ListeToEdit = SelectedListe;
            _windowService.ShowDialog("EditListPage");
        }

        private void ShowAddListDialog()
        {
            var shared = SharedProperties.Instance;
            shared.IsEditListDialog = false;
            shared.ListeToEdit = null;
            _windowService.ShowDialog("EditListPage");
        }

        private void HandleUpdateMessage(UpdateListMessage msg)
        {
            if (msg.ReloadAllData)
            {
                Listen = _handler.GetAllVerzeichnisse();
            }
            else
            {
                Mitglieder = _handler.GetMitgliederInVerzeichnis(msg.IdOfItemToUpdate);
                VisibleFields = _handler.GetFieldVisibilitiesForGivenList(msg.IdOfItemToUpdate);
            }
        }
    }
}