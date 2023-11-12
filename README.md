# wpf_process_manager
Progam powinien wywietlac liste procesow w systemi i zapewniac prosta interacje z nimi (20%)

Wymagania:

    lista (odswieżanie na zyczenie+automatyczne co konfigurowalny czas+zatrzymywanie odswieżania)
    lista procesów powinna wspierać sortowanie i filtrowanie procesów
    Dla wybranego procesu
        pokazywane szczegóły (skalary i listy  -> scenariusz master/detail)
        Możliwość zmiany priorytetu, zabicia
        wyswietlac toolfip z jakas informacja (np. linia polecenia uzyta do uruchomienia)
    Proszę sprobowac odizolowac prezentacje od logiki np. przez zaimplementowanie MVVM (na piechote ew. przez framework) - nalezy pokazac koncepcje UserControl, DataTemplate, Command (dla izolacji modelu i prezentacji)
